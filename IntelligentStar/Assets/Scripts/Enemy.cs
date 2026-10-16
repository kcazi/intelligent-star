using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP;

    public Rigidbody2D rb;

    [SerializeField] public GameObject bullet;

    private void OnCollisonEnter2D(Collider other)
    {
        //TODO: NOT WORKING ADD COLLIOSON  
        // we have to check the tags with what its colliding with instead otherwise when enemy collide with anything, it will die :)      
        HP -= 1;
        Debug.Log(HP);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(HP <= 0){

        }
    }
}
