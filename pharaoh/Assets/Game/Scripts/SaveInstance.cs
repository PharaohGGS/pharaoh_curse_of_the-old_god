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

    private readonly int MOVING_BLOCK_PARSE_INDEX = 12;

    [HideInInspector] public ulong instanceID;
    public Type type = Type.None;

    private void Awake()
    {
        instanceID = ulong.Parse(gameObject.name.Substring(MOVING_BLOCK_PARSE_INDEX));

        Load();
    }

    private void Load()
    {
        // Load this object instance from save data depending on data type
        switch (type)
        {
            case Type.Enemy:
                // Load this enemy state
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
