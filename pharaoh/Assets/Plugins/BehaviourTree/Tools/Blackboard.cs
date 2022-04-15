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

        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

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
            object obj = null;
            bool dontHaveValue = !ContainsData(key) || !_data.TryGetValue(key, out obj) || obj is not T;
            value = dontHaveValue ? default : (T)(obj);
            return !dontHaveValue;
        }

        public T GetData<T>(string key)
        {
            return !ContainsData(key) || !_data.TryGetValue(key, out var obj) || obj is not T typedValue
                ? default : typedValue;
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

        public void ClearAllData()
        {
            _data.Clear();
            debugData.Clear();
        }

        private void SetupListDebug()
        {
            debugData.Clear();
            var keyData = _data.Keys.ToList();
            foreach (var key in keyData)
            {
                var value = _data[key];
                if (value == null) continue;
                debugData?.Add(new Data
                {
                    key = key,
                    type = value.GetType().ToString(),
                    value = value.ToString(),
                });
            }
        }
    }
}