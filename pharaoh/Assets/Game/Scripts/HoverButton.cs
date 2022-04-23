using UnityEngine;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour
{

    public bool interactable = true;

    [Header("Color Change")]
    public Color defaultColor;
    public Color hoverColor;
    public Color disabledColor;

    [Header("Size Change")]
    public float defaultSize;
    public float hoverSize;

    [Space(10f)]
    public TMPro.TMP_Text tmp;
    public Image hoverImage;

    private void OnEnable()
    {
        tmp.color = interactable ? defaultColor : disabledColor;
        tmp.fontSize = defaultSize;
    }

    public void Hover()
    {
        if (interactable)
        {
            tmp.color = hoverColor;
            tmp.fontSize = hoverSize;
        }
        hoverImage.enabled = interactable ? true : false;
    }

    public void Exit()
    {
        if (interactable)
        {
            tmp.color = defaultColor;
            tmp.fontSize = defaultSize;
            hoverImage.enabled = false;
        }
    }

    public void UpdateDisplay()
    {
        tmp.color = interactable ? defaultColor : disabledColor;
        tmp.fontSize = defaultSize;
    }

}
