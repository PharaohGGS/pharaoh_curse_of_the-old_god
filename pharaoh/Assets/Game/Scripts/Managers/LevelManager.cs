using DesignPatterns;

public class LevelManager : MonoSingleton<LevelManager>
{
    public string currentRoom; // Stores the current room scene name
}
