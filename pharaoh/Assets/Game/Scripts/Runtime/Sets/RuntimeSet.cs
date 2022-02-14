using System.Collections.Generic;
using UnityEngine;

namespace Pharaoh.Gameplay.Sets
{
    /// <summary>
    /// https://github.com/roboryantron/Unite2017/blob/master/Assets/Code/Sets/RuntimeSet.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        private readonly List<T> _items = new List<T>();

        private List<string> _debugItems = new List<string>();

        public void Add(T item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                _debugItems.Add(item.ToString());
            }
        }

        public void Remove(T item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                _debugItems.Remove(item.ToString());
            }
        }
    }
}