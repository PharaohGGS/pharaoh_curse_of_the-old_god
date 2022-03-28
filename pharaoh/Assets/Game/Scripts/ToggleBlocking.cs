using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ToggleBlocking : MonoBehaviour
{


    public bool displayMesh = true;
    public int groundLayer = 10;
    private List<MeshRenderer> meshRendererList;

    private void Start()
    {
        meshRendererList = GetComponentsInChildren<MeshRenderer>().ToList();
        if (!displayMesh)
        {
            meshRendererEnabler(displayMesh);
        }
    }

    private void OnEnable()
    {
        meshRendererEnabler(true);
    }

    private void OnDisable()
    {
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
