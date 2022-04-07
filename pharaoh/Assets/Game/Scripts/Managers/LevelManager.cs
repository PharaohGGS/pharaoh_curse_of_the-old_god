using Pharaoh.Managers;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string currentRoom; // Stores the current room scene name
    public UnityAction roomChanged;

    public void ChangeRoom(string room)
    {
        currentRoom = room;
        roomChanged?.Invoke();
    }

}
