using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ToggleBlocking : MonoBehaviour
{


    public bool displayMesh;
    public int groundLayer = 10;
    private List<MeshRenderer> meshRendererList;

    private void Start()
    {
        meshRendererList = GetComponentsInChildren<MeshRenderer>().ToList();
        Debug.Log("Start");
        if (!displayMesh)
        {
            meshRendererEnabler(displayMesh);
        }
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        /*GetComponentsInChildren<MeshRenderer>().ToList().ForEach((m) =>
        {
            if (m.gameObject.layer == groundLayer)
            {
                m.enabled = true;
            }
        });*/
        meshRendererEnabler(true);
    }

    private void OnDisable()
    {
        /*GetComponentsInChildren<MeshRenderer>().ToList().ForEach((m) =>
        {
            if (m.gameObject.layer == groundLayer)
            {
                m.enabled = false;
            }
        });*/
        meshRendererEnabler(false);
    }

    private void meshRendererEnabler(bool isDisplayed)
    {
        meshRendererList.ForEach((m) =>
        {
            if (m.gameObject.layer == groundLayer)
            {
                m.enabled = isDisplayed;
            }
        });
    }
}
