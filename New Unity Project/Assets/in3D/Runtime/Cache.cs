using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Networking;

namespace in3D.AvatarsSDK
{
    public struct CachedRequest
    {
        public UnityWebRequest request;
        public UnityWebRequestAsyncOperation asyncOperation;
    }

    public class Cache : Alteracia.Patterns.Manager<Cache>
    {
        private Mutex _mutex = new Mutex();
        public Dictionary<Uri, CachedRequest> cachedRequests = new Dictionary<Uri, CachedRequest>();

        // TODO ADD TRY GET CACHED OBJECT
        
        public static CachedRequest Cached(Uri url)
        {
            Instance._mutex.WaitOne();
        
            CachedRequest request;
            bool isCached = Instance.cachedRequests.ContainsKey(url);
        
            if (isCached)
            {
                request = Instance.cachedRequests[url];
            }
            else
            {
                request = new CachedRequest();
            }
        
            Instance._mutex.ReleaseMutex();

            return request;
        }

        public static void AddToCache(Uri url, UnityWebRequest request, UnityWebRequestAsyncOperation asyncOperation)
        {
            CachedRequest cachedRequest = new CachedRequest() { request = request, asyncOperation = asyncOperation };
        
            Instance._mutex.WaitOne();
        
            Instance.cachedRequests.Add(url, cachedRequest);
        
            Instance._mutex.ReleaseMutex();
        }
    }
}