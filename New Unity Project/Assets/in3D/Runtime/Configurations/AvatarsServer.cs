using UnityEngine;

namespace in3D.AvatarsSDK.Configurations
{
    /// <summary>
    /// Single file for server configuration.
    /// Using for backend requests.
    /// </summary>
    [CreateAssetMenu(fileName = "AvatarsServer", menuName = "in3D/AvatarsServer", order = 0)]
    [System.Serializable]
    public class AvatarsServer : ScriptableObject
    {
        [SerializeField][Tooltip("Vendor id or name")]
        private string vendor;
        public string Vendor => string.IsNullOrEmpty(vendor) ? "in3d" : vendor;
        [SerializeField][Tooltip("Visibility option: all, web, mobile")]
        private string visibility;
        [SerializeField][Tooltip("Production or staging")]
        private bool staging;

        public AvatarsServer()
        {
            UserAvatar = new UserAvatarApiV2(this);
        }

        public string Url => "https://" + (staging ? "p.app.gsize.io" : "app.gsize.io");
        public string Append => $"vendor={Vendor}&visibility={visibility}";
        public string ToJson => JsonUtility.ToJson(this);
        public IUserAvatarApi UserAvatar { get; }
    }
}
