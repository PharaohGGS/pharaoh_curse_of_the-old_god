using UnityEngine;

namespace in3D.AvatarsSDK.Configurations
{
    public class ServerConfigurator : Alteracia.Patterns.ScriptableObjects.ConfigurableController<ServerConfigurator, AvatarsServer>
    {
        protected override void OnConfigurationRead()
        {
            //in3D.Plugins.Server.Initiate(configuration);
        }
    }
}
