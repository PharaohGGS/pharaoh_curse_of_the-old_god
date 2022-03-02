using System.Threading.Tasks;
using Alteracia.Web;
using UnityEngine;

namespace in3D.AvatarsSDK.Configurations
{
    public interface IUserConfig
    {
        public string UserAppend { get; }
        public string[] AuthorizationHeader { get; }
        public string[] UserHeader { get; }
        public string[] DeviceHeader { get; }
    }
    
    [CreateAssetMenu(fileName = "UserConfiguration", menuName = "in3D/UserConfiguration", order = 1)]
    [System.Serializable]
    public class UserConfiguration : ScriptableObject, IUserConfig
    {
        [System.Serializable] 
        private struct RefreshTokenJson
        {
            public string refresh_token;
        }
        
        [System.Serializable]
        public class SocialLoginJson
        {
            public string token;
            public string vendor;
        }

        [SerializeField][Tooltip("Insert if you have")]
        private string device_id;

        [SerializeField][Tooltip("Insert if you have")]
        private string user_id;
        [HideInInspector][SerializeField]
        private string id;
        public string UserId => string.IsNullOrEmpty(id) ? user_id : id;
        
        [HideInInspector][SerializeField]
        private string access_token;
        [HideInInspector][SerializeField]
        private string refresh_token;
        
        [HideInInspector][SerializeField]
        private string email;
        [HideInInspector][SerializeField]
        private string full_name;


        public async Task<bool> Login(AvatarsServer server, string login, string password)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", login);
            form.AddField("password", password);
            
            using(var req = await Requests.Post($"{server.Url}/v1/users/login/jwt", form))
            {
                if (!req.Success()) return false;

                JsonUtility.FromJsonOverwrite(
                    req.downloadHandler.text.FormatJsonText(typeof(UserConfiguration)), this);
            }
            return true;
        }

        public async Task<bool> SocialLogin(AvatarsServer server, string token, string vendor)
        {
            var message = JsonUtility.ToJson(new SocialLoginJson { token = token, vendor = vendor });

            using (var req =
                await Requests.PostJson($"{server.Url}/v1/users/social_login/jwt", message))
            {
                if (!req.Success()) return false;

                JsonUtility.FromJsonOverwrite(
                    req.downloadHandler.text.FormatJsonText(typeof(UserConfiguration)), this);
            }
            return true;
        }

        public void ConnectWithQr(string uuid) // TODO handle ending
        {
            user_id = uuid;
            access_token = uuid;
            refresh_token = "";
        }

        internal async Task<bool> Refresh(AvatarsServer server)
        {
            if (string.IsNullOrEmpty(refresh_token)) return false;
            
            var message = JsonUtility.ToJson(new RefreshTokenJson { refresh_token = this.refresh_token });
            
            using( var req = await Requests.PostJson($"{server.Url}/v1/users/refresh/jwt", 
                message, AuthorizationHeader))
            {
                if (!req.Success()) return false;

                JsonUtility.FromJsonOverwrite(
                    AltJson.FormatJsonText(req.downloadHandler.text, typeof(UserConfiguration)), this);
            }
            return true;
        }

        public async Task WhoAmI(AvatarsServer server)
        {
            using( var req = await Requests.Get($"{server.Url}/v1/users/me", AuthorizationHeader))
            {
                if (!req.Success()) return;

                JsonUtility.FromJsonOverwrite(req.downloadHandler.text.FormatJsonText(typeof(UserConfiguration)), this);
            }
        }

        public async Task<bool> DeleteMySelf(AvatarsServer server)
        {
            using (var req = await Requests.Delete($"{server.Url}/v1/users/me", AuthorizationHeader))
            {
                return req.Success();
            }
        }
        
        public string UserAppend => user_id.Equals(access_token) ? "" : $"user_id={user_id}";

        internal string[] LoginHeader => new[] { "Content-Type", "application/x-www-form-urlencoded" };
        public string[] AuthorizationHeader => new[] { "Authorization", $"Bearer {access_token}" };
        public string[] UserHeader => string.IsNullOrEmpty(UserId) ? null : new[] { "user-id", UserId };
        public string[] DeviceHeader => string.IsNullOrEmpty(device_id) ? null : new[] { "device-id", $"{device_id}" };
        public string ToJson => JsonUtility.ToJson(this);

        private void OnDestroy()
        {
            access_token = null;
            refresh_token = null;
        }
    }
}
