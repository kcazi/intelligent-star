using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, moveSpeed*Time.deltaTime);
    }
}
