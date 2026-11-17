
using UnityEngine;
using UnityEngine.InputSystem;

public class SoftwareSpriteCursor : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            // Move the crosshair to the mouse position
            transform.position = Mouse.current.position.ReadValue();
        }
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }
}