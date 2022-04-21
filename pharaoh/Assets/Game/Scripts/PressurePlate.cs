using Pharaoh.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using AudioManager = Pharaoh.Managers.AudioManager;

public class PressurePlate : MonoBehaviour
{

    private Transform _mesh;
    private int _pressCount; //counts the amount of bodies pressing the trigger

    public LayerMask whatCanTrigger;
    public UnityEvent OnPress = new UnityEvent();
    public UnityEvent OnRelease = new UnityEvent();

    private void Awake()
    {
        _mesh = transform.Find("Skin");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (whatCanTrigger.HasLayer(collision.gameObject.layer))
        {
            if (_pressCount == 0)
            {
                // Invoke the event while the first body triggers the plate
                OnPress.Invoke();
                AudioManager.Instance.Play("PlateOn");

                _mesh.localScale = new Vector3(0.66f, 0.125f, 0.66f); //squish the plate to make it easier to see
            }
            _pressCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (whatCanTrigger.HasLayer(collision.gameObject.layer))
        {
            if (_pressCount == 1)
            {
                // Invoke the event while the last body leaves the plate
                OnRelease.Invoke();
                AudioManager.Instance.Play("PlateOff");

                _mesh.localScale = new Vector3(0.66f, 0.25f, 0.66f); //unsquish the plate
            }

            _pressCount--;
        }
    }

    public void TestDebugText(string text)
    {
        Debug.Log(text);
    }

#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = _pressCount > 0 ? new Color(0f, 0.5f, 0f) : Color.red;
        Handles.Label(transform.position - (Vector3.up / 2f), (_pressCount > 0 ? "Triggered" : "Release") + "\nPress Count : " + _pressCount, style);
    }
    
#endif
}
