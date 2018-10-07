using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//THIS IS A SCRIPT FROM CYBERSURFER 3000 GAME, USING FMOD
//THE SCRIPT IS FREE TO USE, AS LONG AS IT FITS WELL IN YOUR PROJECT.
//TELL ME ABOUT YOUR PROJECT, I WOULD LIKE TO KNOW ABOUT IT :)
//GOOD LUCK

public class CriticalHP : MonoBehaviour {

//This class handles the Player's critical health feedback on 
//Cybersurfer 3000 AND SHOULD BE USED ALONG WITH FMOD
    public callSoundFMod CriticalHPBeep;
    public float beepRate = 1;

    private Image activatedImage;

    private float nextTimeToBeep;
    private bool coroutineCalled;
    private bool critical;
    private bool activated;

	//Here we set up some variables
    void Start()
    {
        nextTimeToBeep = 0;
        critical = false;
        activatedImage = GetComponent<Image>();
        activatedImage.enabled = false;
        coroutineCalled = false;
        activated = false;
    }

	//Here we constantly check if the player is at a critical health state
    private void Update()
    {
		//if YES, then we'll call the Blink function to give the neccessary feedbacks
		//if NO, then we just reensure it is NOT on a critical state.
        if (critical && Time.time >= nextTimeToBeep)
        {
            nextTimeToBeep = Time.time + 1f / beepRate;

            Blink();
        }

        if (!critical)
        {
            critical = false;
            activatedImage.enabled = false;
            coroutineCalled = false;
            activated = false;
            StopSound(CriticalHPBeep.eventPath, CriticalHPBeep.target);
        }
    }

	//This function gives the player the feedbacks they need
    private void Blink()
    {
		//An image that blinks...
        activatedImage.enabled = !activated;
        activated = !activated;
        if (!activated)
        {
            StopSound(CriticalHPBeep.eventPath, CriticalHPBeep.target);
        }
		//... and a sound that pops up along with the blinking image
        if (activated)
        {
            CallSound(CriticalHPBeep.eventPath, CriticalHPBeep.target);
        }
        coroutineCalled = false;
    }

	//The script doesn't work alone, the "critical" boolean needs to
	//be set externally, calling the "toggleCritical" function
    public void toggleCritical(bool isCrit)
    {
        critical = isCrit;
    }

	//Both functions below are the FMOD part of this script.
    private void StopSound(string EventPath, GameObject target)
    {
		//We create an emitter
        StudioEventEmitter fmodEvent;
		//Then we define the desired objecto to it
        fmodEvent = target.GetComponent<FMODUnity.StudioEventEmitter>();
		//And here I've worked with the Sound Guy. We decided that when
		//the parameter is on 0 it is turned off...
        fmodEvent.SetParameter("off_on", 0);
    }

    private void CallSound(string EventPath, GameObject target)
    {
        StudioEventEmitter fmodEvent;
        fmodEvent = target.GetComponent<FMODUnity.StudioEventEmitter>();
		//... and when it is on 1 the sound plays.
        fmodEvent.SetParameter("off_on", 1);
    }
}
