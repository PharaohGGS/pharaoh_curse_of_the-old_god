using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class CameraManager : PersistantMonoSingleton<CameraManager>
{
    public GameObject player;
    public GameObject vcamFollowOffset;
    public Vector3 cameraOffset;
}
