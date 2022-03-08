using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class PressurePlate : MonoBehaviour
{

    private int _pressCount; //counts the amount of bodies pressing the trigger

    public LayerMask whatCanTrigger;
    public UnityEvent OnPress = new UnityEvent();
    public UnityEvent OnRelease = new UnityEvent();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (whatCanTrigger == (whatCanTrigger | (1 << collision.gameObject.layer)))
        {
            if (_pressCount == 0) OnPress.Invoke(); //invoke the event while the first body triggers the plate
            _pressCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (whatCanTrigger == (whatCanTrigger | (1 << collision.gameObject.layer)))
        {
            if (_pressCount == 1) OnRelease.Invoke(); //invoke the event while the last body leaves the plate
            _pressCount--;
        }
    }

    public void TestDebugText(string text)
    {
        Debug.Log(text);
    }

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = _pressCount > 0 ? new Color(0f, 0.5f, 0f) : Color.red;
        Handles.Label(transform.position - (Vector3.up / 2f), (_pressCount > 0 ? "Triggered" : "Released") + "\nPress Count : " + _pressCount, style);
    }

}
