using in3D.AvatarsSDK.Configurations;
using UnityEngine;

public class AvatarFromScanIdConfigurator : AvatarConfigurator
{
    [SerializeField] private AvatarsServer server;
    [SerializeField] private UserConfiguration user;
    
    public void SetAvatarScanID(string scanId)
    {
        this.SetConfiguration($"{{\"scan_id\": \"{scanId}\"}}", true, false);
        GetAvatar();
    }

    private async void GetAvatar()
    {
       var success = await this.configuration.GetAvatarFromScan(server, user);
       if (!success)
       {
           Debug.LogError($"Can't get avatar from scan_id");
           return;
       }
       this.ReadConfiguration();
    }
}
