using System.Threading.Tasks;
using in3D.AvatarsSDK.Configurations;
using UnityEngine;
using UnityEngine.Events;

public class ReadLastUserAvatar : MonoBehaviour
{
    [SerializeField] private AvatarsServer server;
    [SerializeField] private UserConfiguration user;
    [SerializeField] private AvatarConfiguration avatar;

    public UnityEvent onAvatarIdRead = new UnityEvent();

    public void ReadLastAvatar()
    {
        Read();
    }
    
    async void Read()
    {
        await Task.Yield();
        var ids = await server.UserAvatar.GetAvatarsIds(user);
        avatar.SetUp(user.UserId, "", ids[ids.Length - 1], avatar.Format);
        onAvatarIdRead.Invoke();
    }
}
