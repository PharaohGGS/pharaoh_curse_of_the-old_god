using System.Threading.Tasks;
using Alteracia.Web;
using UnityEngine;

namespace in3D.AvatarsSDK.Configurations
{
    /// <summary>
    /// Avatar Configuration, not necessary. Use for conversion from scan id to avatar id.
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarConfiguration", menuName = "in3D/AvatarConfiguration", order = 2)]
    [System.Serializable]
    public class AvatarConfiguration : ScriptableObject
    {
        /// <summary>
        /// Id of user.
        /// </summary>
        public string UserId { get; private set; }

        [SerializeField][Tooltip("Avatar id, if you have only scan_id use GetAvatarFromScan Method")]
        private string id;
        /// <summary>
        /// Id of avatar.
        /// </summary>
        public string AvatarId => id;

        [SerializeField] [Tooltip("Format of avatar model")]
        private ModelFormat format = ModelFormat.Glb;
        /// <summary>
        /// Format of avatar model
        /// </summary>
        public ModelFormat Format => format;
        
        [SerializeField][Tooltip("Insert if you have, use GetAvatarFromScan Method to get avatar id")]
        private string scan_id;
        public string ScanId => scan_id;
        
        public string ToJson => JsonUtility.ToJson(this);

        /// <summary>
        /// Pass main configuration setups.
        /// </summary>
        /// <param name="userId">Id of user - owner of avatar</param>
        /// <param name="scanId">Id of scan</param>
        /// <param name="avatarId">Id of avatar</param>
        /// <param name="modelFormat">Glb, Vto,..</param>
        public void SetUp(string userId, string scanId, string avatarId, ModelFormat modelFormat)
        {
            this.UserId = userId;
            this.scan_id = scanId;
            this.id = avatarId;
            this.format = modelFormat;
        }

        /// <summary>
        /// Get Avatar from Scan. Use if scan id is defined in avatar configuration.
        /// </summary>
        /// <param name="server">Server Configuration</param>
        /// <param name="user">User Configuration</param>
        /// <returns>true if avatar for scan exists and user is authorized</returns>
        public async Task<bool> GetAvatarFromScan(AvatarsServer server, UserConfiguration user)
        {
            return await GetAvatarFromScan(server, user, this.scan_id);
        }
        
        /// <summary>
        /// Get avatar id from scan id.
        /// </summary>
        /// <param name="server">Server Configuration</param>
        /// <param name="user">User Configuration</param>
        /// <param name="scanId">Scan id</param>
        /// <returns>true if avatar for scan exists and user is authorized</returns>
        public async Task<bool> GetAvatarFromScan(AvatarsServer server, UserConfiguration user, string scanId)
        {
            using (var req = await Alteracia.Web.Requests.Post(
                $"{server.Url}/v2/user_avatars/get_by_scan/{scanId}", user.AuthorizationHeader))
            {
                if (!req.Success())
                {
                    Debug.Log("Cant get avatarId from scan");
                    return false;
                }
            
                JsonUtility.FromJsonOverwrite(
                    req.downloadHandler.text.FormatJsonText(typeof(AvatarConfiguration)), this);
            }
        
            return true;
        }
    }
}
