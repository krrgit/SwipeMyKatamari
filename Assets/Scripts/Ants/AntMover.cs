using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMover : MonoBehaviour
{
    [SerializeField] private float antSpeed = 2;
    
    [SerializeField] private List<GameObject> ants = new List<GameObject>();

    private PropCollector propCollector;

    // Start is called before the first frame update
    void Awake()
    {
        propCollector = PropCollector.Instance;
    }

    private void OnEnable()
    {
        propCollector.myCollectProp += RemoveAnt;
    }
    
    private void OnDisable()
    {
        propCollector.myCollectProp -= RemoveAnt;
    }

    void RemoveAnt(float newRadius, Prop prop)
    {
        if (ants.Contains(prop.gameObject))
        {
            ants.Remove(prop.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ants.Count; ++i)
        {
            ants[i].transform.position += antSpeed * Time.deltaTime * -ants[i].transform.forward;
        }
    }
    
}
