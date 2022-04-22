
using DesignPatterns;
using UnityEngine;

namespace Pharaoh.Managers
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public GameObject player;
        public GameObject vcamFollowOffset;
        public Vector3 cameraOffset;
    }
}
