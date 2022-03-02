using System.Threading.Tasks;
using UnityEngine;

using Alteracia.Web;

namespace Alteracia.Patterns.ScriptableObjects
{
    [CreateAssetMenu(fileName = "WebConfigurationReader", menuName = "ConfigurationReader/WebReader", order = 0)]
    [System.Serializable]
    public class WebConfigurationReader : ConfigurationReader
    {
        [SerializeField] private bool local;
        [SerializeField] private string urlToHost;
        
        public override async Task ReadConfigFile(ScriptableObject configurable)
        {
            using (var req = await Requests.Get(
                System.IO.Path.Combine(local ? Application.absoluteURL : urlToHost, configurable.name)))
            {
                if (!req.Success()) return;
                JsonUtility.FromJsonOverwrite(req.downloadHandler.text, configurable);
            }
        }
    }
}
