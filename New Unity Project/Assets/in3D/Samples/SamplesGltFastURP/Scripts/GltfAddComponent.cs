#if GLTFAST_UNITY
using GLTFast;
#endif
using in3D.Avatars;
using in3D.AvatarsSDK;
using in3D.AvatarsSDK.Configurations;
using UnityEngine;

public class GltfAddComponent : MonoBehaviour
{
    [SerializeField] private AvatarsServer server;
    [SerializeField] private UserConfiguration user;
    [SerializeField] private AvatarConfiguration avatar;
    
    [SerializeField] private RuntimeAnimatorController controller;

    public void AddGltfComponent()
    {
        LoadAvatar();
    }

    private async void LoadAvatar()
    {
        var urls = await server.UserAvatar.GetAvatarUrls(user, avatar.Format, avatar.AvatarId);

#if GLTFAST_UNITY
        GltfAsset gltfast;
        if (avatar.Format == ModelFormat.Glb)
        {
            gltfast = this.gameObject.AddComponent<GltfHumanAsset>();
            ((GltfHumanAsset)gltfast).controller = controller;
        }
        else if (avatar.Format == ModelFormat.Vto)
            gltfast = this.gameObject.AddComponent<GltfAsset>();
        else return;
        
        gltfast.url = urls.GetMainUrl();
#else
        Debug.LogWarning("No gltf loader! Please install <a href=\"https://github.com/atteneder/glTFast\">gltfast</a> ");
#endif
    }
}
