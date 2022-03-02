using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddUser : MonoBehaviour
{
    public ReadLastUserAvatar button;
    public GltfAddComponent component;

    public void AddPrefab()
    {
        var but = Instantiate(button.gameObject, this.transform);
        var transf = Instantiate(component.gameObject);

        but.GetComponent<ReadLastUserAvatar>().onAvatarIdRead.AddListener(transf.GetComponent<GltfAddComponent>().AddGltfComponent);
    }
    
}
