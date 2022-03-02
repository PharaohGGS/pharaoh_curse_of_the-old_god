using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttt : MonoBehaviour
{
    public GameObject player;
   public void Resp()
    {
       CapsuleCollider myCollider = player.GetComponent<CapsuleCollider>();
        myCollider.radius = 10f; // or whatever radius you want.
    }
}
