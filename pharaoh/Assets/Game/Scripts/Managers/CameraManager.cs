using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class CameraManager : PersistantMonoSingleton<CameraManager>
{
    public Transform playerTransform;
}
