using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMover : MonoBehaviour
{
    [SerializeField] private float antSpeed = 2;

    [SerializeField] private GameObject[] ants;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ants.Length; ++i)
        {
            ants[i].transform.position += antSpeed * Time.deltaTime * -ants[i].transform.forward;
        }
    }
}
