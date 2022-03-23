using UnityEngine;
using UnityEditor;
using Pharaoh.Tools;

public class Door : MonoBehaviour
{

    private BoxCollider2D _boxCollider;
    private MeshRenderer _meshRenderer;
    private int _activePlates = 0;
    private int _blockers = 0;
    private Animator _animator;

    [Tooltip("An inverted door means an active pressure plate will close it.")]
    public bool inverted = false;

    [Tooltip("Clipping Material")]
    public Material _material;
    public GameObject door;
    public GameObject firstScarab;
    public GameObject secondScarab;

    [Tooltip("Layers able to block the door from closing")]
    public LayerMask whatCanBlockDoor;

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
        // The door closes if unblocked
        _animator.SetBool("IsOpen", IsOpen() || IsBlocked());
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

    // Returns whether the door is blocked or not
    public bool IsBlocked()
    {
        return _blockers > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (whatCanBlockDoor.HasLayer(collision.gameObject.layer))
        {
            _blockers++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (whatCanBlockDoor.HasLayer(collision.gameObject.layer))
        {
            _blockers--;
            RefreshState();
        }
    }

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = IsOpen() ? Color.green : Color.red;
        Handles.Label(transform.position, IsOpen() ? "Opened" : "Closed", style);
        style.normal.textColor = IsBlocked() ? Color.red : Color.green;
        Handles.Label(transform.position + Vector3.up, IsBlocked() ? "Blocked" : "Unblocked", style);
    }

}
