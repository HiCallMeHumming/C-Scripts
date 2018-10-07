using System.Collections;
using UnityEngine;
using FMOD.Studio;

//THIS IS A SCRIPT FROM CYBERSURFER 3000 GAME, USING FMOD
//THE SCRIPT IS FREE TO USE, AS LONG AS IT FITS WELL IN YOUR PROJECT.
//TELL ME ABOUT YOUR PROJECT, I WOULD LIKE TO KNOW ABOUT IT :)
//GOOD LUCK

public class enemyBehaviour : MonoBehaviour {
	
	/*This class handles all enemies' basic behaviours, 
	like his health, how he moves and its feedbacks,
	requiring FMOD to play sound feedbacks. */
	
    [Header("Variáveis comuns")]
    public float Vida;
    public float velocidadeInimigo;
    [Tooltip("Make sure it has ONLY 1 object with that tag on the scene. If it has multiple objects, make it the first object with that tag on the Inspector.")]
    public string tagAlvo;
    public float danoColisao;
    [Range(0, 1)]
    public float multiplicadorEstagnado;
    [Range(-1, 1)]
    public float retardoPlayer;
    [Range(0, 1)]
    public float invincibilityDuration;
    public bool segue;
    [Tooltip("Se seguir, a que distancia olhara?")]
    public float distanciaLookAt = 40;
    [Tooltip("The lesser, the stronger the slow.")]
    public ParticleSystem[] feedbacksVisuais;
    public callSoundFMod sonsTomaDano;
    public callSoundFMod sonsMorte;
    public Material[] enemyMaterials;

    [Header("Variáveis de inimigos atiradores")]
    [Tooltip("Set this to TRUE if the enemy is ranged")]
    public bool atirador = false;
    public float velocidadeTiro;
    public float taxaDeTiro = 2f;
	public float distanciaInicioAtirar;
    public GameObject Bala;
    public float danoTiro;
    public Transform saidaDoTiro;
    public ParticleSystem tiroFeedbackVisual;
    public callSoundFMod tiroFeedbacksSonoro;

    private float EndInvincibility;
    private PlayerControllerPC playerController;
    private Renderer rend;
    private Camera cam;
    private Transform camT;
    private float Stagnate;
    private float nextTimeToFire = 0f;
    private float startPosY;
    private float startPosX;
    private Transform Target;
    private Collider col;
    private bool doesCollisionDamage;
    private bool invincible = false;
    private bool dead = false;
	private bool readyToFire = false;

	
	//Here we give life to our private variables
    void Start ()
    {
        if (danoColisao > 0 || danoColisao < 0)
        {
            doesCollisionDamage = true;
        }
        else
        {
            doesCollisionDamage = false;
        }
        dead = false;
		readyToFire = false;
        Target = GameObject.FindGameObjectWithTag(tagAlvo).GetComponent<Transform>();

        startPosY = transform.position.y;
        startPosX = transform.position.x;

        rend = GetComponent<Renderer>();
        rend.enabled = false;
        col = GetComponent<Collider>();
        col.enabled = false;
        
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camT = cam.GetComponent<Transform>();

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerPC>();
		
		/*Here is a cool trick a friend of mine taught me:
		To check some informations only at some desired
		frames. More on this on the coroutine itself */
        StartCoroutine(CheckDistEveryThirdFrame());
    }
	
	
	//As we are using Rigidbody to move the enemy, we 
	//have to use FixedUpdate to detect collision properly.
	void FixedUpdate ()
    {
        Stagnate = playerController.actSpeedZ;
        Parado();
        checaVida();
		
		/*Here I am just manually occluding the rendering of the enemy
		so we don't waste a lot of processing rendering him. The same
		applies to his collider. */
        if (rend.enabled == false && !dead)
            Occlusion();
        
        if(rend.enabled == true && velocidadeInimigo > 0 && !dead)
            Segue();

        if(atirador && Time.time >= nextTimeToFire && !dead && readyToFire)
        {
            nextTimeToFire = Time.time + 1f / taxaDeTiro;
            Atira();
        }
    }

	/*Okay, so this is the trick my friend taught me
	I'll be calling a different type of function called
	Coroutines. This Coroutine is called every third frames
	and I needed it because I didn't wanted to waste processing
	checking the distance between the enemy and the player every
	frame, so I'll be checking every third frame! */
    private IEnumerator CheckDistEveryThirdFrame()
    {
		/*I am checking the distance twice: this one
		is to ensure the enemy collides with the player, 
		applying damage ( playerHealth.tomaDano() )... */
        if (Vector3.Distance(transform.position, Target.position) <= 1)
        {
            Target.gameObject.GetComponent<playerHealth>().tomaDano(danoColisao);
            Target.gameObject.GetComponent<PlayerControllerPC>().Retardar(retardoPlayer);
            CallSound(sonsTomaDano.eventPath, sonsTomaDano.target);
            gameObject.SetActive(false);
        }
		
		//And this one tells if the enemy will start shooting
		//only if he is an enemy that shoots projectiles
        if (atirador && !readyToFire && Vector3.Distance(transform.position, Target.position) <= distanciaInicioAtirar)
        {
			readyToFire == true;
        }
        yield return 3;
        StartCoroutine(CheckDistEveryThirdFrame());
    }

	//Just a function to check the enemies' health.
	//being at 0 or below, it dies.
    void checaVida()
    {
        if (Vida <= 0 && !dead)
        {
            rend.enabled = false;
            col.enabled = false;
            StartCoroutine(Morre());
        }
    }

    void Occlusion()
    {
        if (Vector3.Distance(camT.position, transform.position) <= cam.farClipPlane + 5)
            {
            rend.enabled = true;
            col.enabled = true;
        }
    }

	/*On this function we'll be checking distance
	again, but on this case it is not always.
	If he sufficiently near the player, it will move
	directly towards him, looking at the player. */
    void Segue()
    {
        if (segue)
        {
            if (Vector3.Distance(transform.position, Target.position) >= distanciaLookAt)
            {
                transform.Translate(0, 0, (Stagnate + velocidadeInimigo) * Time.deltaTime);
                transform.position = new Vector3(startPosX, startPosY, transform.position.z);
            }
            else
            {
                transform.Translate(0, 0, (Stagnate + velocidadeInimigo) * Time.deltaTime);
            }
        }
    }

	//If the enemy is not rendered, it will move slower
    void Parado()
    {
        if (!dead)
        {
            transform.LookAt(Target);
        }
        transform.Translate(0, 0, - multiplicadorEstagnado * (Stagnate * Time.deltaTime), Space.World);
    }

	//Function called to shoot
    void Atira()
    {
        GameObject tiroVirus1;
        tiroVirus1 = Instantiate(Bala, saidaDoTiro.position, Quaternion.identity) as GameObject;
		//How we call a sound using FMOD
        CallSound(tiroFeedbacksSonoro.eventPath, tiroFeedbacksSonoro.target);
    }
    
	//Function called on damaging the player by touch
    private void OnCollisionEnter(Collision collision)
    {
        if (doesCollisionDamage)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
				//if he collided with the Player, then apply damage and feedbacks
                collision.gameObject.GetComponent<playerHealth>().tomaDano(danoColisao);
                collision.gameObject.GetComponent<PlayerControllerPC>().Retardar(retardoPlayer);
                CallSound(sonsTomaDano.eventPath, gameObject);
                gameObject.SetActive(false);
            }
        }
    }

	//Function called when the enemy takes damage
    public void tomaDano(float dano)
    {
        if (!invincible && !dead)
        {
			//Calling feedbacks
            feedbacksVisuais[1].Play();
            CallSound(sonsTomaDano.eventPath, sonsTomaDano.target);
            Vida -= dano;
			
			/*I chose to give the enemy an invincibility 
			time after taking damage, so it doesn't die 
			way too fast */
            StartCoroutine(InvincibilityAfterDamage(invincibilityDuration));
        }
    }

	/*Here we give our invincibility feedbacks
	and turn off some variables, like his 
	collision box. */
    private IEnumerator InvincibilityAfterDamage(float duration)
    {
        col.enabled = false;
        rend.material = enemyMaterials[1];
        invincible = true;
        yield return new WaitForSeconds(duration);
        invincible = false;
        if (!dead)
        {
            col.enabled = true;
            rend.material = enemyMaterials[0];
        }
    }

	//Both functions below are the FMOD part of this script.
    public void CallSound(string EventPath, GameObject target)
    {
		//We create an emitter
        EventInstance fmodEvent = FMODUnity.RuntimeManager.CreateInstance(EventPath);
		//Then we define the desired objecto to it
        fmodEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(target.transform.position));		//And here I've worked with the Sound Guy. We decided that when
		//and here is how you make a sound not loop on FMOD
        fmodEvent.start();
        fmodEvent.release();
    }
	
	/*If we need to stop the enemy sound (so it doesn't continue
	playing after the player went to the Game Over screen) then
	this is the function to go */
    private void StopSound(string EventPath, GameObject target)
    {
        target.GetComponent<EventInstance>().release();
    }

	/*On this Coroutine we'll be applying the Death state on 
	the enemy. We disable the rendering and collision, also 
	giving some feedbacks and then we disable the gameObject. */
    private IEnumerator Morre()
    {
        rend.enabled = false;
        col.enabled = false;
        feedbacksVisuais[0].Play();
        dead = true;
        CallSound(sonsMorte.eventPath, sonsMorte.target);

        yield return new WaitForSeconds(3);
		
        gameObject.SetActive(false);
    }
}