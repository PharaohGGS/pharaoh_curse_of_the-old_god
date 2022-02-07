using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourTree.Tools
{
    [Serializable]
    public class Blackboard
    {
        private Dictionary<string, dynamic> _datas = new Dictionary<string, dynamic>();
        private Dictionary<string, System.Type> _dataTypes = new Dictionary<string, System.Type>();
        [SerializeField] private List<string> keyData = new List<string>();
        [SerializeField] private List<string> typeData = new List<string>();
        [SerializeField] private List<string> valueData = new List<string>();

        public dynamic GetData(string key)
        {
            return _datas.TryGetValue(key, out dynamic value) ? value : null;
        }

        public void SetData(string key, dynamic value)
        {
            if (_datas.ContainsKey(key))
            {
                _datas[key] = value;
                _dataTypes[key] = value.GetType();
            }
            else
            {
                _datas.Add(key, value);
                _dataTypes.Add(key, value.GetType());
            }

            SetupListDebug();
        }

        public bool ClearData(string key)
        {
            bool result = false;
            if (_datas.ContainsKey(key))
            {
                _datas.Remove(key);
                _dataTypes.Remove(key);
                result = true;
            }

            SetupListDebug();

            return result;
        }

        private void SetupListDebug()
        {
            keyData = _datas.Keys.ToList();
            valueData.Clear();
            typeData.Clear();
            _datas.Values.ToList().ForEach(v =>
            {
                valueData.Add(v.ToString());
                typeData.Add(v.GetType().Name);
            });
        }
    }
}