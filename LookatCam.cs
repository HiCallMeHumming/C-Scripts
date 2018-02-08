using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatCam : MonoBehaviour {
    private Camera c;

    private void Start()
    {
        c = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    //Put this script in an object to make it look at the scene's Main Camera
    void Update () {
        transform.LookAt(transform.position + c.transform.rotation * Vector3.back, c.transform.rotation * Vector3.up);
	}
}
