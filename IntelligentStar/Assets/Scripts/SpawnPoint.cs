using System.Linq;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public WaveManager WM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WM.spawnPoints.Add(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
