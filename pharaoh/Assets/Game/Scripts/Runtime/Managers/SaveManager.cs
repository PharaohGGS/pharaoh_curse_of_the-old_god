using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DesignPatterns;
using UnityEditor;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    // public static void SaveEnemy(Enemy enemy)
    // {
    //     BinaryFormatter formatter = new BinaryFormatter();
    //     string path = Application.persistentDataPath + "/" + enemy.id + ".save";
    //     FileStream stream = new FileStream(path, FileMode.Create);
    //
    //     EnemyData data = new EnemyData(enemy);
    //     
    //     Debug.Log(data.isAlive);
    //     
    //     formatter.Serialize(stream, data);
    //     stream.Close();
    // }
    //
    // public static EnemyData LoadEnemy(GlobalObjectId id)
    // {
    //     string path = Application.persistentDataPath + "/" + id + ".save";
    //     if (File.Exists(path))
    //     {
    //         BinaryFormatter formatter = new BinaryFormatter();
    //         FileStream stream = new FileStream(path, FileMode.Open);
    //
    //         EnemyData data = formatter.Deserialize(stream) as EnemyData;
    //         stream.Close();
    //         return data;
    //     }
    //     Debug.Log("Couldn't load save: " + path);
    //     return null;
    // }
}
