using UnityEngine;

namespace Pharaoh.Tools
{
    public enum MessageType
    {
        Log, Warning, Error
    }

    public class LogHandler : Singleton<LogHandler>
    {
        public delegate void DSendMessage(string message, MessageType type);
        public static event DSendMessage OnSendMessage;

        public void SendMessage(string message, MessageType type)
        {
            OnSendMessage?.Invoke(message, type);
        }
    }
}