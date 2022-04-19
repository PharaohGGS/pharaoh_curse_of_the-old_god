using Pharaoh.Gameplay.Components;
using UnityEngine;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

public class SaveInstance : MonoBehaviour
{
    public enum Type
    {
        None,
        Enemy,
        MovingBlock
    };

    private readonly int ENEMY_PARSE_INDEX = 6;
    private readonly int MOVING_BLOCK_PARSE_INDEX = 12;

    [HideInInspector] public ulong instanceID;
    public Type type = Type.None;

    private void Awake()
    {
        // Prevents error while debugging
        if (SaveDataManager.Instance == null)
            return;

        if (type == Type.Enemy)
            instanceID = ulong.Parse(gameObject.name.Substring(ENEMY_PARSE_INDEX));
        else if (type == Type.MovingBlock)
            instanceID = ulong.Parse(gameObject.name.Substring(MOVING_BLOCK_PARSE_INDEX));

        Load();
    }

    private void OnDestroy()
    {
        // Prevents error while debugging
        if (SaveDataManager.Instance == null)
            return;

        // Saves the instance when unloaded
        Save();
    }

    private void Load()
    {
        // Load this object instance from save data depending on data type
        switch (type)
        {
            case Type.Enemy:
                // Load this enemy state
                bool state;
                SaveDataManager.Instance.LoadEnemyState(instanceID, out state);
                gameObject.SetActive(state);
                break;

            case Type.MovingBlock:
                // Load this block position if previously saved, otherwise keep its original position
                Vector3 position;
                if (SaveDataManager.Instance.LoadBlockPosition(instanceID, out position))
                    gameObject.transform.position = position;
                break;

            case Type.None:
            default:
                break;
        }
    }

    public void Save()
    {
        // Save this object instance to save data depending on data type
        switch (type)
        {
            case Type.Enemy:
                // Save this enemy state
                SaveDataManager.Instance.SaveEnemyState(instanceID, !GetComponent<HealthComponent>().isDead);
                break;

            case Type.MovingBlock:
                // Save this block position
                SaveDataManager.Instance.SaveBlockPosition(instanceID, transform.position);
                break;

            case Type.None:
            default:
                break;
        }
    }

}
