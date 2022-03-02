using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Patterns
{
    public abstract class QueueManager<T, T0> : Manager<T> where T : Manager<T>
    {
        [SerializeField] 
        protected int maxTry = 0;
        
        [SerializeField]
        protected List<T0> queue = new List<T0>(); // TODO use Queue net
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        
        public void AddToQueue(T0 item, int position)
        {
            queue.Insert(position, item);
            
            if (queue.Count == 1)
                ExecuteQueue();
        }

        public void RemoveFromQueue(Func<T0, bool> predicate)
        {
            bool exists = queue.Any(predicate);
            if (!exists) return;
            queue.Remove(queue.First(predicate));
        }

        private async void ExecuteQueue()
        {
            bool success = false;
            int trying = 0;

            object current = null;
            _cancelTokenSource = new CancellationTokenSource();
            
            while (true)
            {
                if (_cancelTokenSource.Token.IsCancellationRequested)
                    return;
                
                // Remove loaded or failed model from queue
                if (current != null && (success || trying > this.maxTry))
                {
                    if (queue.Contains((T0)current))
                        queue.Remove((T0)current);
                    trying = 0;
                }
                
                // Finish if queue is empty
                if (queue.Count == 0) break;
                
                trying++;

                // Create tasks
                current = queue[0];
                success = await Execution((T0)current);
                // Pause for dispose
                await Task.Yield();
                
                if (!success) continue;
                
                trying = 0;

                AfterExecution((T0)current);
            }
        }

        protected abstract Task<bool> Execution(T0 current);

        protected abstract void AfterExecution(T0 current);

        private void OnDestroy()
        {
            queue.Clear();
            _cancelTokenSource.Cancel();
        }
    }
}
