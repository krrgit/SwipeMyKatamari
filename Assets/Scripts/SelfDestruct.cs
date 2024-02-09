using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifetime = 3;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
