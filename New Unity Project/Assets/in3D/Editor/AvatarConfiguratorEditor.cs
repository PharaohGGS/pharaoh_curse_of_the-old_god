using UnityEditor;

namespace in3D.AvatarsSDK.Editor
{
    [CustomEditor(typeof(Configurations.AvatarConfigurator))]
    public class AvatarConfiguratorEditor : Alteracia.Patterns.Editor.Configurable<Configurations.AvatarConfigurator, Configurations.AvatarConfiguration>
    {
        protected override void Initiate()
        {
        }
    }
}
