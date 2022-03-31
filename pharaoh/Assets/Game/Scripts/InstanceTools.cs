using UnityEngine;
using UnityEditor;

public class InstanceTools
{
    [MenuItem("Instances/Generate Blocks Instance IDs")]
    private static void GenerateBlocksInstanceIDs()
    {
        ulong id = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MovingBlock"))
        {
            go.GetComponent<MovingBlock>().instanceID = id++;
        }

        GameObject.FindObjectOfType<Pharaoh.Managers.SaveDataManager>().GetComponent<Pharaoh.Managers.SaveDataManager>().MOVINGBLOCKSCOUNT = (uint)id;
        Debug.Log("Counted " + id + " moving blocks.");
    }
}
