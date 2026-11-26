using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public List<Transform> spawnPoints; //List of spawnpoints in game
    public int spawnerIndex; //Will be set to call a certain spawn point in the list
    public int waveNumber; //What what is the player on
    public GameObject enemyPrefab;
    public float spawnInterval = 0.5f;
    public float spawnTime = 0f;
    public int spawnedEnemies;
    public bool canSpawn;
    public int waveIncrememnt = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoints = new List<Transform>();
        spawnedEnemies = 0;
        waveNumber = 0;
        canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime;

        if((spawnTime > spawnInterval) && canSpawn)
        {
            spawnEnemy();
            spawnTime = 0;
        }
    }

    void waveComplete()
    {
        //Checkes if all enemies are destroyed and if so then the wave will finsih
        if (true)
        {
            waveNumber++;
        }

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
        spawnedEnemies++;

        //Wave 1 = 8 zombies, 2 = 10, 3 = 14, 4 = 22,...
        if (spawnedEnemies >= (6 + Mathf.Pow(waveIncrememnt, waveNumber)))
        {
            canSpawn = false;
        }
    }
}
