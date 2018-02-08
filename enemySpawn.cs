using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour {

    //Defines the spawn and enemies possibilities
    public Transform[] spawners;
    public GameObject[] enemies;
	
	//This void calls a random enemy at a random spawn point
    public void SpawnNewEnemy()
    {
        Instantiate(enemies[Random.Range(0, enemies.Length)], spawners[Random.Range(0, spawners.Length)].position, Quaternion.identity);
    }
}
