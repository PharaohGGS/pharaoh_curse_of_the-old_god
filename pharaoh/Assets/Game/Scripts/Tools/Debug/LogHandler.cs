using UnityEngine;

namespace Pharaoh.Tools.Debug
{
    public enum MessageType
    {
        Log, 
        Warning, 
        Error
    }

    public class LogHandler
    {
        public delegate void DSendMessage(string message, MessageType type);
        public static event DSendMessage OnSendMessage;

        public static void SendMessage(string message, MessageType type)
        {
#if UNITY_EDITOR
            if (LogConsole.Instance == null)
            {
                UnityEngine.Debug.LogWarning($"LogConsole not instantiated");
            }
#endif

            OnSendMessage?.Invoke(message, type);
        }
    }
}