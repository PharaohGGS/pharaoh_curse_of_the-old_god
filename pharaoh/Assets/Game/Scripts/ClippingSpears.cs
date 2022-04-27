using UnityEngine;

public class ClippingSpears : MonoBehaviour
{
    private void Awake()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_Clip", transform.position.y + 1.1f);
        GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
    }
}
