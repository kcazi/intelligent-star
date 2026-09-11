using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    
    public float moveSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float direction = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            direction = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            direction = 1f;
        }

        if (Keyboard.current.spaceKey.isPressed)
        {
            Debug.Log("Space Pressed");
        }

        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;
    }
}
