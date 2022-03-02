using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Alteracia.Patterns
{
    public class Collector<T> : MonoBehaviour where T : UnityEngine.Object
    {
        [SerializeField]
        protected List<T> collection;

        public int Length => collection.Count;

        protected void CollectChildren(bool rewrite = true, Func<T, bool> predicate = null)
        {
            var range = predicate != null 
                ? this.transform.GetComponentsInChildren<T>(true).Where(predicate) 
                : this.transform.GetComponentsInChildren<T>(true);
            CollectRange(range, rewrite);
        }
        
        protected void CollectFromScene(bool rewrite = true, Func<T, bool> predicate = null)
        {
            var range = predicate != null ? FindObjectsOfType<T>().Where(predicate) : FindObjectsOfType<T>();
            
            CollectRange(range, rewrite);
        }

        protected void CollectRange(IEnumerable<T> range, bool rewrite = false)
        {
            if (range == null) return;
            
            if (rewrite || collection == null)
            {
                collection?.Clear();
                collection = range.ToList();
            }
            else
                collection?.AddRange(range.ToList());
        }

        public void Collect(T item)
        {
            collection.Add(item);
        }
        
        public void Remove(T item)
        {
            if (!collection.Contains(item)) return;
            collection.Remove(item);
        }

        public T GetAtIndex(int index)
        {
            if (index < 0 || index > collection.Count - 1) return null;
            return collection[index];
        }
        
        public void ExecuteForLast(UnityAction<T> action)
        {
            if (collection[collection.Count - 1] == null) return;
            action.Invoke(collection[collection.Count - 1]);
        }

        public void ExecuteFor(int index, UnityAction<T> action)
        {
            if (index < 0 || index >= collection.Count || collection[index] == null) return;
            action.Invoke(collection[index]);
        }

        public void ExecuteForEach(UnityAction<T> action)
        {
            foreach (var obj in collection)
            {
                action.Invoke(obj);
            }
        }
        
        public void ExecuteForEach(UnityAction<T, int> action)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                action.Invoke(collection[i], i);
            }
        }
        
        public void ExecuteForRange(int start, int end, UnityAction<T> action)
        {
            int s = (int)Mathf.Clamp(start, 0, collection.Count);
            int f = (int)Mathf.Clamp(end, s, collection.Count);
            
            for (int i = s; i < f; i++)
            {
                action.Invoke(collection[i]);
            }
        }
    }
}
