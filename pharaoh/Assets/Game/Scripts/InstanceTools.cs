#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;
using System.Collections.Generic;

public class InstanceTools
{
    private static List<string> GetAllBuildScenes()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
        }
        return scenes;
    }

    [MenuItem("Instances/Load Build Scenes")]
    private static void LoadBuildScenes()
    {
        List<string> scenes = GetAllBuildScenes();
        foreach (string s in scenes)
        {
            if (!SceneManager.GetSceneByPath(s).isLoaded)
                EditorSceneManager.OpenScene(s, OpenSceneMode.Additive);
        }
    }

    [MenuItem("Instances/Generate Enemies Instance IDs")]
    private static void GenerateEnemiesInstanceIDs()
    {
        ulong id = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            go.name = "Enemy_" + id++;
        }

        foreach (string s in GetAllBuildScenes())
        {
            EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(s));
        }

        Debug.Log("Counted " + id + " enemies.");
    }

    [MenuItem("Instances/Generate Blocks Instance IDs")]
    private static void GenerateBlocksInstanceIDs()
    {
        ulong id = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MovingBlock"))
        {
            go.name = "MovingBlock_" + id++;
        }

        foreach (string s in GetAllBuildScenes())
        {
            EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(s));
        }

        Debug.Log("Counted " + id + " moving blocks.");
    }

    [MenuItem("Instances/Unload Build Scenes")]
    private static void UnloadBuildScenes()
    {
        List<string> scenes = GetAllBuildScenes();
        foreach (string s in scenes)
        {
            Scene scene = SceneManager.GetSceneByPath(s);
            if (scene.isLoaded && scene != SceneManager.GetActiveScene())
                EditorSceneManager.CloseScene(scene, false);
        }
    }
}
#endif
