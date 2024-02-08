using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateJumpParticles : MonoBehaviour
{
    [SerializeField] private float startEmitterSize = 3;

    [SerializeField] private float endEmitterSize = 1;

    [SerializeField] private ParticleSystem particleSystem;


    public void StartAnim(float duration)
    {
        StartCoroutine(IAnimate(duration));
    }

    IEnumerator IAnimate(float duration)
    {
        var shape = particleSystem.shape;
        float sizeDelta = startEmitterSize - endEmitterSize;
        shape.radius = startEmitterSize;
        
        particleSystem.Play();
        float timer = duration;

        while (timer > 0)
        {
            shape.radius = endEmitterSize + (sizeDelta * timer/duration);
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shape.radius = endEmitterSize;
    }

    public void StopAnim()
    {
        particleSystem.Clear();
        particleSystem.Stop();
    }
}
