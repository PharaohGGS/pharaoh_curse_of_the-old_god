using System.Threading.Tasks;
using Alteracia.Patterns;
using Alteracia.Patterns.ScriptableObjects;
using UnityEngine;

namespace in3D.AvatarsSDK.Configurations
{
    [CreateAssetMenu(fileName = "AvatarFromScan", menuName = "in3D/Readers/AvatarFromScan", order = 0)]
    [System.Serializable]
    public class AvatarFromScanConfigurationReader : ConfigurationReader
    {
        [SerializeField] private string scanId;
        [SerializeField] private AvatarsServer server;
        [SerializeField] private UserConfiguration user;

        public override async Task ReadConfigFile(ScriptableObject configurable)
        {
            var config = (AvatarConfiguration)configurable;
            await config.GetAvatarFromScan(server, user, scanId);
        }
    }
}
