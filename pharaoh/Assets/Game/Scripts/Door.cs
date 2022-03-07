using UnityEngine;
using UnityEditor;

public class Door : MonoBehaviour
{

    private BoxCollider2D _boxCollider;
    private MeshRenderer _meshRenderer;
    private bool _isOpened = false;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _meshRenderer = transform.Find("Skin").GetComponent<MeshRenderer>();
    }

    public void Open()
    {
        if (!_isOpened)
        {
            _boxCollider.enabled = false;
            _meshRenderer.enabled = false;

            _isOpened = true;
        }
    }

    public void Close()
    {
        if (_isOpened)
        {
            _boxCollider.enabled = true;
            _meshRenderer.enabled = true;

            _isOpened = false;
        }
    }

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = _isOpened ? Color.green : Color.red;
        Handles.Label(transform.position, _isOpened ? "Opened" : "Closed", style);
    }

}
