using UnityEngine;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

public class InstanceLoader : MonoBehaviour
{
    public enum Type
    {
        None,
        Enemy,
        MovingBlock
    };

    public Type type = Type.None;

    private void Awake()
    {
        // Avoid using this script while in editor mode
        if (Application.isEditor)
            enabled = false;

        // Load this object instance from save data depending on data type
        switch (type)
        {
            case Type.Enemy:
                // Load this enemy state
                break;

            case Type.MovingBlock:
                // Load this block position if previously saved, otherwise keep its original position
                Vector3 position;
                if (SaveDataManager.Instance.LoadBlockPosition(GetComponent<MovingBlock>().instanceID, out position))
                    gameObject.transform.position = position;
                break;

            case Type.None:
            default:
                break;
        }
    }
}
