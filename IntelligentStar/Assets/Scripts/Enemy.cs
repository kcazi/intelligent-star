using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float HP;

    public Rigidbody2D rb;

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

    public void TakeDamage(float dmg)
    {
        HP -= dmg;

        if(HP <= 0){
            DespawnEnemy();
        }
    }

    public void DespawnEnemy()
    {
        Destroy(gameObject);
    }
}
