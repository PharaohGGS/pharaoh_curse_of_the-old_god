using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;

public class GameManager : PersistantMonoSingleton<GameManager>
{
    [SerializeField] private GameObject player;

    public GameObject Player => player;
}
