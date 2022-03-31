using UnityEngine;

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
        switch (type)
        {
            case Type.Enemy:
                // Load this enemy state
                break;

            case Type.MovingBlock:
                // Load this block position
                Vector3 position;
                if (Pharaoh.Managers.SaveDataManager.Instance.LoadBlockPosition(GetComponent<MovingBlock>().instanceID, out position))
                    gameObject.transform.position = position;                
                break;

            case Type.None:
            default:
                break;
        }
    }
}
