using UnityEditor;

namespace in3D.AvatarsSDK.Editor
{
    [CustomEditor(typeof(Configurations.UserConfigurator))]
    public class UserConfiguratorEditor : Alteracia.Patterns.Editor.Configurable<Configurations.UserConfigurator, Configurations.UserConfiguration>
    {
        protected override void Initiate()
        {
            
        }
    }
}
