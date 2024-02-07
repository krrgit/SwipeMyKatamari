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

    [Header("Absorb Values")] 
    [SerializeField]private float absorbDur = 1f;

    public delegate void CollectProp(float newRadius, string propName);

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
        if (!collision.gameObject.CompareTag("Prop")) return;
        

        // print("Touched prop");
        
        var prop = collision.gameObject.GetComponent<Prop>();
        if (prop)
        {
            if (prop.Volume <= volume * maxPercCollectable)
            {
                AddPropToCollection(prop, collision.GetContact(0).thisCollider);
                print("collect: " + prop.Volume + " | max: " + (volume * maxPercCollectable));
            }
            else
            {
                print("Too big. Prop: " + prop.Volume + " | Limit: " + volume * maxPercCollectable);
            }
            
        }
    }

    void AddPropToCollection(Prop prop, Collider myCollider)
    {
        prop.transform.parent = collectionParent;
        prop.gameObject.layer = ballLayer;
        prop.DestroyRigidbody();

        StartCoroutine(IAbsorbObject(prop.transform));
        UpdateVolume(prop.Volume);
                
        myCollectProp?.Invoke(radius, prop.PropName);
        SoundManager.Instance.PlayClip("PickupProp");
    }

    IEnumerator IAbsorbObject(Transform prop)
    {
        float delta = Mathf.Max(prop.localPosition.magnitude - radius * 0.5f,0) * (Time.deltaTime / absorbDur);
        float tempMag = prop.localPosition.magnitude;
        float duration = 0;
        while (tempMag > radius * 0.5f)
        {
            tempMag -= delta;
            prop.localPosition = tempMag * prop.localPosition.normalized;
            duration += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        // print("finish absorb. r: " +radius + " | lp: " + prop.localPosition.magnitude + " | dur: " + duration);   
    }
}
