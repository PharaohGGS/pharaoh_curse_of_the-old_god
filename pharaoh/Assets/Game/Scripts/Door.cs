using UnityEngine;
using UnityEditor;
using Pharaoh.Tools;
using AudioManager = Pharaoh.Managers.AudioManager;
using System.Collections;

public class Door : MonoBehaviour
{

    private BoxCollider2D _boxCollider;
    private MeshRenderer _meshRenderer;
    private int _activePlates = 0;
    private int _blockers = 0;
    private Animator _animator;
    private int _closedDoorLayer;
    private int _openedDoorLayer;

    [Header("Info")]
    private Vector3 _startPos;
    private float _timer;
    private Vector3 _randomPos;

    [Header("Settings")]
    [Range(0f, 2f)]
    public float _time = 0.2f;
    [Range(0f, 2f)]
    public float _distance = 0.1f;
    [Range(0f, 0.1f)]
    public float _delayBetweenShakes = 0f;

    [Header("Behavior")]

    [Tooltip("An inverted door means an active pressure plate will close it.")]
    public bool inverted = false;
    [Tooltip("Is the door placed horizontally")]
    public bool horizontal = false;
    [Tooltip("Layers able to block the door from closing")]
    public LayerMask whatCanBlockDoor;

    [Header("Models")]

    [Tooltip("Clipping Material")]
    public GameObject door;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _meshRenderer = transform.Find("Skin").GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();

        _closedDoorLayer = LayerMask.NameToLayer("Ground");
        _openedDoorLayer = LayerMask.NameToLayer("Ignore Raycast");
        _startPos = transform.position;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_Clip", (horizontal ? transform.position.x + 1.5f : transform.position.y - 1.5f));
        door.GetComponent<MeshRenderer>().SetPropertyBlock(mpb);

        RefreshState();
    }
    private void OnValidate()
    {
        if (_delayBetweenShakes > _time)
            _delayBetweenShakes = _time;
    }

    // Called when a pressure plate is pressed
    public void Open()
    {
        _activePlates++;
        StopAllCoroutines();
        StartCoroutine(Shake());
        //AudioManager.Instance.Play("DoorOpens");
        RefreshState();
    }

    // Called when a pressure plate is released
    public void Close()
    {
        _activePlates--;
        StopAllCoroutines();
        StartCoroutine(Shake());
        //AudioManager.Instance.Play("DoorCloses");
        RefreshState();
    }

    // Refreshes the state of the door, whether it is open or not
    private void RefreshState()
    {
        // The door closes if unblocked
        gameObject.layer = (IsOpen() || IsBlocked()) ? _openedDoorLayer : _closedDoorLayer;
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

    public void EnableCollision()
    {
        _boxCollider.isTrigger = false;
    }

    public void EnableTrigger()
    {
        _boxCollider.isTrigger = true;
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
    private IEnumerator Shake()
    {
        _timer = 0f;

        while (_timer < _time)
        {
            _timer += Time.deltaTime;

            _randomPos = _startPos + (Random.insideUnitSphere * _distance);

            //transform.position = _randomPos;
            door.transform.position = _randomPos;

            if (_delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(_delayBetweenShakes);
            }
            else
            {
                yield return null;
            }
        }

        //transform.position = _startPos;
        door.transform.position = _startPos;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = IsOpen() ? Color.green : Color.red;
        Handles.Label(transform.position, IsOpen() ? "Opened" : "Closed", style);
        style.normal.textColor = IsBlocked() ? Color.red : Color.green;
        Handles.Label(transform.position + Vector3.up, IsBlocked() ? "Blocked" : "Unblocked", style);
    }
#endif
}
