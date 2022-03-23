using UnityEngine;
using UnityEditor;

public class Door : MonoBehaviour
{

    private BoxCollider2D _boxCollider;
    private MeshRenderer _meshRenderer;
    private int _activePlates = 0;
    private Animator _animator;

    [Tooltip("An inverted door means an active pressure plate will close it.")]
    public bool inverted = false;

    [Tooltip("Clipping Material")]
    public Material _material;
    public GameObject door;
    public GameObject firstScarab;
    public GameObject secondScarab;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _meshRenderer = transform.Find("Skin").GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();

        _material.SetFloat("_Clip", transform.position.y - 1.5f);

        door.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(_material);
        firstScarab.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(_material);
        secondScarab.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(_material);

        RefreshState();
    }
    
    // Called when a pressure plate is pressed
    public void Open()
    {
        _activePlates++;

        RefreshState();
    }

    // Called when a pressure plate is released
    public void Close()
    {
        _activePlates--;

        RefreshState();
    }

    // Refreshes the state of the door, whether it is open or not
    private void RefreshState()
    {
        bool isOpen = IsOpen();

        _boxCollider.enabled = !isOpen;
        _meshRenderer.enabled = !isOpen;

        _animator.SetBool("IsOpen", isOpen);
    }

    // Returns whether the door is open or not
    // The door is opened when 1 pressure plate or more are pressed
    // If inverted, the door is openede when 0 pressure plate are pressed
    public bool IsOpen()
    {
        if (!inverted)
            return _activePlates > 0;
        else
            return _activePlates < 1;
    }

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = IsOpen() ? Color.green : Color.red;
        Handles.Label(transform.position, IsOpen() ? "Opened" : "Closed", style);
    }

}
