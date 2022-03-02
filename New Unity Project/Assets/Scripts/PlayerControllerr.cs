using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class PlayerControllerr : MonoBehaviour
{


    private Animator animator;
    public GameObject player;

    void Awake()
    {
        animator = GetComponent<Animator>();
      
    }
    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            // Plays the walking animation - stops all other animations
            animator.SetTrigger("Pharaoh");
        }
        //else if (Input.GetKeyDown("x"))
        //{
        //    // Plays the walking animation - stops all other animations
        //    animator.Play("Attack3");
        //}
    }
}
