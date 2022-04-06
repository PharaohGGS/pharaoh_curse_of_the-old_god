using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInputs : MonoBehaviour
{
    public Image effectImage;
    public Color imagecolor;
    public float speed = 1f;
    public float baseAlpha = 200f;

    void Start()
    {
        imagecolor = effectImage.color;
    }

    private void Update()
    {
        imagecolor.a = ((Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f);
        if(imagecolor.a <= 200)
        {
            effectImage.color = imagecolor;
        }
    }
}
