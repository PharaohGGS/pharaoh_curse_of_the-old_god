using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputs : MonoBehaviour
{
    public GameObject brackets;
    private void OnMouseOver()
    {
        brackets.SetActive(true);
    }
    private void OnMouseExit()
    {
        brackets.SetActive(false);
    }
}
