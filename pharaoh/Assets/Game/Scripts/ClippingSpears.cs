using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingSpears : MonoBehaviour
{
    private void Awake()
    {
        Material originalMat = GetComponent<MeshRenderer>().material;
        Material mat = new Material(originalMat.shader);
        mat.CopyPropertiesFromMaterial(originalMat);
        mat.SetFloat("_Clip", transform.position.y + 1.1f);

        GetComponent<MeshRenderer>().material = mat;
    }
}
