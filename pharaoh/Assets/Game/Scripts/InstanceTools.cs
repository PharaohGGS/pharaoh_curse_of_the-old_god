using UnityEngine;
using UnityEditor;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;

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

        if (Object.FindObjectOfType<SaveDataManager>().TryGetComponent(out SaveDataManager sdm))
            sdm.MOVINGBLOCKSCOUNT = (uint)id;
        else
            Debug.LogWarning("Could not find any SaveDataManager in the current scene.");
        Debug.Log("Counted " + id + " moving blocks.");
    }
}
