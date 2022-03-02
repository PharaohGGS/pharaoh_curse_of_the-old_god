using System.Collections.Generic;

namespace in3D.AvatarsSDK
{
    public class In3dUser
    {
        public Dictionary<string, In3dAvatar> Avatars { get; } = new Dictionary<string, In3dAvatar>();
    }
}
