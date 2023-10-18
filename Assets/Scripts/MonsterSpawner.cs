using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    float currentTime;
    float minTime = 7;
    float maxTime = 10;
    public float createTime;
    public GameObject[] monsters;
    public Transform[] spawnPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        createTime = UnityEngine.Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime > createTime)
        {
            CreateMonster();
            createTime = UnityEngine.Random.Range(minTime, maxTime);
            currentTime = 0;
        }
    }

    private void CreateMonster()
    {
        GameObject monster = monsters[Random.Range(0, monsters.Length)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        monster = Instantiate(monster, spawnPoint.position, spawnPoint.rotation);
    }
}
