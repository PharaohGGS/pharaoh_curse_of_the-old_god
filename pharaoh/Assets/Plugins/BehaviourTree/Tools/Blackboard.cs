using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        #region Debug

        [SerializeField] private List<Data> debugData = new List<Data>();

        #endregion

        public bool ContainsData(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool TryGetData(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        public bool TryGetData<T>(string key, out T value)
        {
            bool found = ContainsData(key);
            value = found ? (T) _data[key] : default;
            return found;
        }

        public T GetData<T>(string key)
        {
            bool isGettingValue = _data.TryGetValue(key, out var value);
            bool isGenericType = value is T;
            return isGettingValue && isGenericType ? (T)value : default;
        }

        public void SetData(string key, object value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
            }
            else
            {
                _data.Add(key, value);
            }

            SetupListDebug();
        }

        public void SetData<T>(string key, T value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
            }
            else
            {
                _data.Add(key, value);
            }

            SetupListDebug();
        }

        public bool ClearData(string key)
        {
            bool result = false;
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
                result = true;
            }

            SetupListDebug();

            return result;
        }

        private void SetupListDebug()
        {
            debugData.Clear();
            var keyData = _data.Keys.ToList();
            foreach (var key in keyData)
            {
                var value = _data[key];
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