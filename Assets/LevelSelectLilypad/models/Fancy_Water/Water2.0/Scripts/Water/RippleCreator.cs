using UnityEngine;
using System.Collections.Generic;

public class RippleCreator : MonoBehaviour
{
    public bool IsReversedRipple;
    public float RippleStrenght = 0.1f;
    public float MaxVelocity = 1.5f;
    public float RandomRipplesInterval = 0;
    public float reversedRippleDelay = 0.2f;
    public GameObject SplashEffect;
    public GameObject SplashEffectMoved;
    public AudioSource SplashAudioSource;

    private int fadeInVelocityLimit = 10;
    private int fadeInVelocity = 1;
    private WaterRipples waterRipples;
    private Vector3 oldPos;
    private float currentVelocity;
    private Transform t;
    private Queue<ReversedRipple> reversedVelocityQueue;
    private float triggeredTime;
    private bool canUpdate;
    private float randomRipplesCurrentTime;
    private bool canInstantiateRandomRipple;
    private GameObject splashMovedInstance;
    private ParticleSystem splashParticleSystem;
    public float splashSizeMultiplier = 2;
	
	void Start ()
	{
	    t = transform;
        reversedVelocityQueue = new Queue<ReversedRipple>();
	}

    private void FixedUpdate()
    {
        if (waterRipples==null)
            return;

        if (RandomRipplesInterval > 0.0001f && Time.time - randomRipplesCurrentTime > RandomRipplesInterval) {
            randomRipplesCurrentTime = Time.time;
            canInstantiateRandomRipple = true;
        }

        if (canUpdate) {
            currentVelocity = ((t.position - oldPos).magnitude / Time.fixedDeltaTime) * RippleStrenght;
            if (currentVelocity > MaxVelocity)
                currentVelocity = MaxVelocity;
            if (IsReversedRipple)
                currentVelocity = -currentVelocity;
            reversedVelocityQueue.Enqueue(new ReversedRipple { Position = t.position, Velocity = -currentVelocity / fadeInVelocity });
            oldPos = t.position;
            waterRipples.CreateRippleByPosition(t.position, currentVelocity / fadeInVelocity);
            if (canInstantiateRandomRipple)
                waterRipples.CreateRippleByPosition(t.position, Random.Range(currentVelocity / 5, currentVelocity));
            
            UpdateMovedSplash();
        }

        if (Time.time - triggeredTime > reversedRippleDelay) {
            var reversedRipple = reversedVelocityQueue.Dequeue();
            if (IsReversedRipple)
                reversedRipple.Velocity = -reversedRipple.Velocity;
            waterRipples.CreateRippleByPosition(reversedRipple.Position, reversedRipple.Velocity); 
            if (canInstantiateRandomRipple)
                waterRipples.CreateRippleByPosition(reversedRipple.Position, Random.Range(reversedRipple.Velocity/5 , reversedRipple.Velocity));
        }
        ++fadeInVelocity;

        if (fadeInVelocity > fadeInVelocityLimit)
            fadeInVelocity = 1;
        if (canInstantiateRandomRipple)
            canInstantiateRandomRipple = false;
    }

    private void OnTriggerEnter(Collider collidedObj)
    {
        var temp = collidedObj.GetComponent<WaterRipples>();
        if (temp!=null) waterRipples = temp;
        else return;

        canUpdate = true;
        reversedVelocityQueue.Clear();
        triggeredTime = Time.time;
        fadeInVelocity = 1;
      
        if(SplashAudioSource!=null) SplashAudioSource.Play();
        if (SplashEffect!=null) {
            var offset = waterRipples.GetOffsetByPosition(t.position);
            offset.x = t.position.x;
            offset.z = t.position.z;
            var splash = Instantiate(SplashEffect, offset, new Quaternion());
            Destroy(splash, 2);
        }
        UpdateMovedSplash();
    }

    void UpdateMovedSplash()
    {
        if (splashMovedInstance != null)
        {
            var offset = waterRipples.GetOffsetByPosition(t.position);
            offset.x = t.position.x;
            offset.z = t.position.z;
            splashMovedInstance.transform.position = offset;
            splashParticleSystem.startSize = currentVelocity * splashSizeMultiplier;
        }
        else if (SplashEffectMoved != null)
        {
            splashMovedInstance = Instantiate(SplashEffectMoved, t.position, new Quaternion()) as GameObject;
            splashMovedInstance.transform.parent = waterRipples.transform;
            var offset = waterRipples.GetOffsetByPosition(t.position);
            offset.x = t.position.x;
            offset.z = t.position.z;
            splashMovedInstance.transform.position = offset;
            splashParticleSystem = splashMovedInstance.GetComponentInChildren<ParticleSystem>();
            splashParticleSystem.startSize = currentVelocity * splashSizeMultiplier;
        }
       
    }

    //void OnTriggerExit()
    //{
    //    waterRipples = null;
    //    canUpdate = false;
    //}

    void OnEnable()
    {
        waterRipples = null;
        canUpdate = false;
        if (splashMovedInstance != null)
        {
            Destroy(splashMovedInstance);
        }
    }

    void OnDisable()
    {
        if (splashMovedInstance!=null) {
            Destroy(splashMovedInstance);
        }
    }


    void OnDestroy()
    {
        if (splashMovedInstance!=null) {
            Destroy(splashMovedInstance);
        }
    }

    class ReversedRipple
    {
        public Vector3 Position;
        public float Velocity;
    }
}