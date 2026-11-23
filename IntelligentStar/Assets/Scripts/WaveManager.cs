using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WaveManager : MonoBehaviour
{
    public List<Transform> spawnPoints; //List of spawnpoints in game
    public int spawnerIndex; //Will be set to call a certain spawn point in the list
    public int waveNumber; //What what is the player on
    public GameObject enemyPrefab;
    public float spawnInterval = 1f;
    public float spawnTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoints = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime;

        if(spawnTime > spawnInterval)
        {
            spawnEnemy();
            spawnTime = 0;
        }
    }

    void waveComplete()
    {
        waveNumber++;
    }

    void spawnEnemy()
    {
        spawnerIndex = Random.Range(0, spawnPoints.Count);
        GameObject Zombo = Instantiate(enemyPrefab, spawnPoints[spawnerIndex].transform.position, Quaternion.identity);
        float size = Random.Range(0.5f, 2.0f);
        Enemy Zed = Zombo.GetComponent<Enemy>();
        Zed.HP = Mathf.Ceil(size * 5f);
        Zed.speed = 2f / size;
        Zombo.transform.localScale *= size;
    }
}
