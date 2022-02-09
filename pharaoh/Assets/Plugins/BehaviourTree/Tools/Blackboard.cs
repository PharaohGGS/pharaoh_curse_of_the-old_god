using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree.Tools
{
    [Serializable]
    public class Blackboard
    {
        [Serializable]
        private struct Data
        {
            public string key;
            public string type;
            public string value;
        }

        private Dictionary<string, object> _datas = new Dictionary<string, object>();

        #region Debug

        [SerializeField] private List<Data> debugData = new List<Data>();

        #endregion

        public object GetData(string key)
        {
            return _datas.TryGetValue(key, out var value) ? value : null;
        }

        public void SetData(string key, object value)
        {
            if (_datas.ContainsKey(key))
            {
                _datas[key] = value;
            }
            else
            {
                _datas.Add(key, value);
            }

            SetupListDebug();
        }

        public bool ClearData(string key)
        {
            bool result = false;
            if (_datas.ContainsKey(key))
            {
                _datas.Remove(key);
                result = true;
            }

            SetupListDebug();

            return result;
        }

        private void SetupListDebug()
        {
            debugData.Clear();
            var keyData = _datas.Keys.ToList();
            foreach (var key in keyData)
            {
                var value = _datas[key];
                debugData.Add(new Data
                {
                    key = key,
                    type = value.GetType().ToString(),
                    value = value.ToString(),
                });
            }
        }
    }
}