using UnityEditor;

namespace in3D.AvatarsSDK.Editor
{
    [CustomEditor(typeof(Configurations.ServerConfigurator))]
    public class ServerConfiguratorEditor : Alteracia.Patterns.Editor.Configurable<Configurations.ServerConfigurator, Configurations.AvatarsServer>
    {
        protected override void Initiate()
        {
            
        }
    }
}
