using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public List<Transform> spawnPoints; //List of spawnpoints in game
    public List<GameObject> zomboList;//List of zombies spawned in
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

        waveComplete();
    }

    void waveComplete()
    {
        //Checkes if all enemies are destroyed and if so then the wave will finsih
        if ((spawnedEnemies >= (6 + Mathf.Pow(waveIncrememnt, waveNumber))) && (zomboList.Count == 0))
        {
            waveNumber++;
            spawnedEnemies = 0;
            canSpawn = true;
        }

    }

    void spawnEnemy()
    {
        spawnerIndex = Random.Range(0, spawnPoints.Count);
        GameObject Zombo = Instantiate(enemyPrefab, spawnPoints[spawnerIndex].transform.position, Quaternion.identity);
        zomboList.Add(Zombo);
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

    public void RemoveZombie(GameObject deadZombie)
    {
        zomboList.Remove(deadZombie);

        Debug.Log("Zombies Left " + zomboList.Count);
    }

    public IEnumerator killAllZombies()
    {
        float elapsed = 0f;

        while (elapsed < 3f)
        {
            if ((zomboList.Count > 0))
            {
                zomboList[0].GetComponent<Enemy>().TakeDamage(10f);
            }

            yield return new WaitForSeconds(0.25f);

            elapsed += 0.25f;
        }
    }
}
