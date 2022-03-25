using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class ToggleBlocking : MonoBehaviour
{
    public int groundLayer = 10;
    /*public bool blockingVisible = true;

    private bool changed = true;

    private void Update()
    {
        if (Input
        {
            blockingVisible = !blockingVisible;
            changed = true;
        }

        if(changed)
        {
            changed = false;
            GetComponentsInChildren<MeshRenderer>().ToList().ForEach((m) =>
            {
                if(m.gameObject.layer == groundLayer)
                {
                    m.enabled = blockingVisible;
                }
            });
        }
    }*/

    private void OnEnable()
    {
        GetComponentsInChildren<MeshRenderer>().ToList().ForEach((m) =>
        {
            if (m.gameObject.layer == groundLayer)
            {
                m.enabled = true;
            }
        });
    }

    private void OnDisable()
    {
        GetComponentsInChildren<MeshRenderer>().ToList().ForEach((m) =>
        {
            if (m.gameObject.layer == groundLayer)
            {
                m.enabled = false;
            }
        });
    }
}
