using System;
using DesignPatterns;
using UnityEngine;

namespace Pharaoh.Tools.Debug
{
    public class LogConsole : MonoSingleton<LogConsole>
    {
        [SerializeField] private Color logColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color errorColor = Color.red;

        private void OnEnable()
        {
            LogHandler.OnSendMessage += SendMessage;
        }

        private void OnDisable()
        {
            LogHandler.OnSendMessage -= SendMessage;
        }

        private void SendMessage(string message, MessageType type)
        {
#if UNITY_EDITOR
            switch (type)
            {
                case MessageType.Log:
                    UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>{message}</color>");
                    break;
                case MessageType.Warning:
                    UnityEngine.Debug.LogWarning($"<color=#{ColorUtility.ToHtmlStringRGB(warningColor)}>{message}</color>");
                    break;
                case MessageType.Error:
                    UnityEngine.Debug.LogError($"<color=#{ColorUtility.ToHtmlStringRGB(errorColor)}>{message}</color>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
#endif
        }
    }
}