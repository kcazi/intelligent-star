using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float HP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
