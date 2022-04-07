#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using SaveDataManager = Pharaoh.Managers.SaveDataManager;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SceneTools
{

    private static readonly string SCENE_PATH = "Assets/Game/Scenes/Blocking/Scenes by room/";

    // Loads the SceneMerged scene as well as all the game scenes
    [MenuItem("Scenes/Load Game Scenes")]
    private static void LoadGameScenes()
    {
        var allScenes = new DirectoryInfo(SCENE_PATH).GetFiles("*.unity", SearchOption.AllDirectories);

        EditorSceneManager.OpenScene(SCENE_PATH + "SceneMerged.unity", OpenSceneMode.Single);

        foreach (var file in allScenes)
        {
            if (Regex.IsMatch(file.Name, @"^\d+"))
                EditorSceneManager.OpenScene(file.FullName, OpenSceneMode.Additive);
        }
    }

}
#endif
