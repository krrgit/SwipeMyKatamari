using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntTurner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.Contains("Ant")) return;
        
        other.transform.rotation = transform.rotation;
        other.transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
    }
}
