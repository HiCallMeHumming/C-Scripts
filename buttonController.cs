using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonController : MonoBehaviour {
	
	/* This script will need 2 materials for a visual feedback 
	 * if the button if turned On or Off*/
	
    [HideInInspector]
    public bool Switch = false;
    public Material OFF;
    public Material ON;
    public Light feedbackLight;
    private Color cOFF = new Color(0.86f, 0.49f, 0.64f);
    private Color cON = new Color(0.48f, 0.76f, 0.56f);

    public GameObject[] Disables;
    public GameObject[] Enables;

    public void Awake()
    {
        Switch = false;
        feedbackLight.color = cOFF;
    }

    public void Update () {
        if (Switch == true)
        {
            gameObject.GetComponent<Renderer>().material = ON;
            feedbackLight.color = cON;

            foreach(GameObject toDisable in Disables)
            {
                toDisable.SetActive(false);
                Debug.Log("Desativou!");
            }
            foreach (GameObject toEnable in Enables)
            {
                toEnable.SetActive(true);
            }
        }
        if(Switch == false)
        {
            gameObject.GetComponent<Renderer>().material= OFF;
            feedbackLight.color = cOFF;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            Switch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            Switch = false;
        }
    }
}
