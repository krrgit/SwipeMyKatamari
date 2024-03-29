using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropCollector : Singleton<PropCollector>
{
    [SerializeField] private float radius = 1;
    [SerializeField] private float maxPercCollectable = 0.03f;
    [SerializeField] private float volume;
    [SerializeField] private int ballLayer = 6;
    [SerializeField] private Transform collectionParent;
    [SerializeField] private Transform managerParent;
    [SerializeField] private Collider sphereCollider;
    [SerializeField] private float impulseSoundThreshold = 10; // threshold to play hit sound

    [Header("Absorb Values")] 
    [SerializeField]private float absorbDur = 1f;

    [SerializeField] private Transform GroundCheck;
    public delegate void CollectProp(float newRadius, Prop prop);

    public CollectProp myCollectProp;

    // 1. Collide with ball
    // 2. Smoothly move into ball until ~halfway in it
    // 3. True for objects colliding with objects
    
    // 1 Collider
    // 1. Ball collides with object
    // 2. Object moves into ball until dist of CoM of object to center of ball = radius
    
    
    // Size limit = half of radius
    private void Start()
    {
        ComputeVolume();
    }

    void ComputeVolume()
    {
        volume = (4f / 3f) * Mathf.PI * radius * radius * radius;
    }

    void UpdateVolume(float addVolume)
    {
        volume += addVolume;
        radius = Mathf.Pow(volume * 3f / (4f * Mathf.PI), 1f / 3f);

        managerParent.localScale = radius * Vector3.one;
    }

    public bool CanCollectProp(Prop prop)
    {
        return prop.Volume <= volume * maxPercCollectable;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Prop")) return;
        
        // print("Touched prop");
        
        var prop = other.GetComponent<Prop>();
        if (prop)
        {
            if (prop.Volume <= volume * maxPercCollectable)
            {
                AddPropToCollection(prop, null);
                print("collect: " + prop.Volume + " | max: " + (volume * maxPercCollectable));
            }
            else
            {
                print("Too big. Prop: " + prop.Volume + " | Limit: " + volume * maxPercCollectable);
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Prop"))
        {
            print("coll impuls: " + collision.impulse.magnitude);
            if (collision.impulse.magnitude > impulseSoundThreshold)
            {
                SoundManager.Instance.PlayClip("Hit");
            }
            return;
        }

        var prop = collision.gameObject.GetComponent<Prop>();
        if (prop)
        {
            if (prop.Volume <= volume * maxPercCollectable)
            {
                AddPropToCollection(prop, collision.GetContact(0).thisCollider);
                UpdateGroundCheck();
                print("collect: " + prop.Volume + " | max: " + (volume * maxPercCollectable));
            }
            else
            {
                print("Too big. Prop: " + prop.Volume + " | Limit: " + volume * maxPercCollectable);
            }
            
        }
    }

    void UpdateGroundCheck()
    {
        Vector3 groundCheckPos = GroundCheck.localPosition;
        groundCheckPos.y = -radius;
        GroundCheck.localPosition = groundCheckPos;
    }

    void AddPropToCollection(Prop prop, Collider myCollider)
    {
        prop.gameObject.layer = ballLayer;
        prop.transform.parent = transform;
        prop.DestroyRigidbody();

        StartCoroutine(IAbsorbObject(prop));
        UpdateVolume(prop.Volume);
                
        myCollectProp?.Invoke(radius, prop);
        SoundManager.Instance.PlayClip("PickupProp");
    }

    IEnumerator IAbsorbObject(Prop prop)
    {
        // prop.ToggleCollider(false);

        float delta = Mathf.Max(prop.transform.localPosition.magnitude - radius * 0.5f,0) * (Time.deltaTime / absorbDur);
        float tempMag = prop.transform.localPosition.magnitude;
        float duration = 0;
        while (tempMag > radius * 0.5f)
        {
            tempMag -= delta;
            prop.transform.localPosition = tempMag * prop.transform.localPosition.normalized;
            duration += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        // prop.ToggleCollider(true);

        // print("finish absorb. r: " +radius + " | lp: " + prop.localPosition.magnitude + " | dur: " + duration);   
    }
}
