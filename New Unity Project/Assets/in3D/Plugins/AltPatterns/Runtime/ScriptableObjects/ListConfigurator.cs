using System.Collections.Generic;
using UnityEngine;

namespace Alteracia.Patterns.ScriptableObjects
{
    public class ListConfigurator : MonoBehaviour
    {
        [SerializeField]
        private ConfigurationReader reader;
        [SerializeField]
        private List<ScriptableObject> list = new List<ScriptableObject>();

        private void Start()
        {
            foreach (var so in list)
            {
                reader.ReadConfigFile(so);
            }
        }
    }
}
