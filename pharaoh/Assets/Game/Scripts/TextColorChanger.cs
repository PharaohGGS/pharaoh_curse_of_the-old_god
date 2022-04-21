using UnityEngine;

public class TextColorChanger : MonoBehaviour
{

    [Header("Color Change")]
    public Color defaultColor;
    public Color hoverColor;

    [Header("Size Change")]
    public float defaultSize;
    public float hoverSize;

    [Space(10f)]
    public TMPro.TMP_Text tmp;

    private void OnEnable()
    {
        tmp.color = defaultColor;
        tmp.fontSize = defaultSize;
    }

    public void Hover()
    {
        tmp.color = hoverColor;
        tmp.fontSize = hoverSize;
    }

    public void Exit()
    {
        tmp.color = defaultColor;
        tmp.fontSize = defaultSize;
    }

}
