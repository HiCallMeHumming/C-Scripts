using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyHealth : MonoBehaviour {

    [Range(0f,1f)]
    public float maxHP;
    public GameObject HPBar;
    public Image HPSlider;

    private float actHP;
    private enemySpawn enemySpawnScript;

	/*This script works along with my "enemySpawn" script.
	 *Make sure you have an object in the scene with the tag "SpawnManager" */
    private void Awake()
    {
        enemySpawnScript = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent
                        <enemySpawn>();
    }

    void Start()
    {
        actHP = maxHP;

        anim = gameObject.GetComponent<Animator>();
        anim.SetFloat("Health", actHP);

        HPBar.SetActive(false);
		
		if (actHP <= 0)
			Death();

    }

    //Updates actual HP's values to health bar
    void Update ()
    {
        HPSlider.fillAmount = actHP / maxHP;
	}

    //Applies the damage taken to gameObject's actHP
    public void tomaDano(float damage)
    {
        if (!HPBar.activeInHierarchy)
            HPBar.SetActive(true);
				
        actHP -= damage;
        actHP = Mathf.Clamp(actHP, 0, maxHP);
    }

    /*What happens when it dies
	 * as this script works along with enemySpawn, when the
	 * enemy dies, it resets its hp values and teleports to
	 * one of the spawns in the scene and spawns another
	 * enemy (line 66)*/
    void Death()
    {
        HPBar.SetActive(false);
        actHP = maxHP;
        HPSlider.fillAmount = actHP / maxHP;
        transform.position = enemySpawnScript.spawners[Random.Range(0, enemySpawnScript
                        .spawners.Length)].transform.position;

        enemySpawnScript.SpawnNewEnemy();
    }

}
