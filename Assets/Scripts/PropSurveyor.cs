using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSurveyor : MonoBehaviour
{
    [SerializeField] private int collectableLayer;
    [SerializeField] private int nonCollectableLayer;

    private PropCollector propCollector;

    private void Awake()
    {
        propCollector = PropCollector.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Prop")) return;

        Prop prop = other.GetComponent<Prop>();

        if (prop)
        {
            if (!propCollector.CanCollectProp(prop))
            {
                prop.AddRigidbody();
            }
        }
    }
}
