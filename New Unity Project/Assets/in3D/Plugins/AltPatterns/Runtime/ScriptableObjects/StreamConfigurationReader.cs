using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Patterns.ScriptableObjects
{
    [CreateAssetMenu(fileName = "StreamConfigurationReader", menuName = "ConfigurationReader/StreamReader", order = 0)]
    [System.Serializable]
    public class StreamConfigurationReader : ConfigurationReader
    {
        public override async Task ReadConfigFile(ScriptableObject configurable)
        {
            string path = Path.Combine(Application.streamingAssetsPath, configurable.name);
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, configurable);
            await Task.Yield();
        }
    }
}
