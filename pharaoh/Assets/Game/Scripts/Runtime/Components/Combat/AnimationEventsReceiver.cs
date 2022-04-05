using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsReceiver : MonoBehaviour
{

    public UnityAction sheatheSwords;
    public UnityAction drawSwords;

    // Used by the "Sheathing Swords" animation
    public void SheatheSwords()
    {
        sheatheSwords?.Invoke();
    }

    // Used by the "Drawing Swords" animation
    public void DrawSwords()
    {
        drawSwords?.Invoke();
    }

}
