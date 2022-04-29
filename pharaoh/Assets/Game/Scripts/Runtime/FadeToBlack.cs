using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{

    private RawImage _blackScreen;

    [Tooltip("Duration of the fade animation")]
    public float fadeDuration = 3f;

    private void Awake()
    {
        _blackScreen = GetComponent<RawImage>();
        _blackScreen.color = Color.black;
        _blackScreen.canvasRenderer.SetAlpha(0f);
    }

    public void Fade()
    {
        _blackScreen.canvasRenderer.SetAlpha(1f);
        _blackScreen.CrossFadeAlpha(0f, fadeDuration, false);
    }

}
