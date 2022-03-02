using System.Threading.Tasks;
using UnityEngine;

namespace in3D.AvatarsSDK.Configurations
{
    public class LoginUserConfigurator : UserConfigurator
    {
        [SerializeField] private string login;
        [SerializeField] private string password;

        [SerializeField] private AvatarsServer server;

        public void LoginAndReadConfiguration(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
            
            LoginAndReadAsync();
        }

        // Can't call async in UnityEvents - call async method inside
        public void LoginAndReadConfiguration()
        {
            LoginAndReadAsync();
        }
        
        async void LoginAndReadAsync()
        {
            await Login();
            this.ReadConfiguration();
        }

        private async Task Login()
        {
            bool success = await configuration.Login(server, login, password);
            if (!success)
            {
                Debug.Log("Can't login user", this.gameObject);
            }
        }
    }
}
