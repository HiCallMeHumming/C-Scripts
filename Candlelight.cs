using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candlelight : MonoBehaviour
{
    public float maxIntensity;
    public float minIntensity;

    private Light lt;
	private float value;
	private bool reverse;

	void Start ()
	{
		lt = gameObject.GetComponent<Light> ();
	}

    //Blowing wind effect on the applied object with a Light component
	void Update ()
	{
		if (!reverse) {
			if (value < maxIntensity) {
				value += Time.deltaTime;
			} else
				reverse = true;
		}

		else {
			if (value > minIntensity) {
				value -= Time.deltaTime;
			} else
				reverse = false;
		}

		lt.intensity = value;	
	}
}
