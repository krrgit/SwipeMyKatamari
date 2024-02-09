using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateJumpParticles : MonoBehaviour
{
    [SerializeField] private float startEmitterSize = 3;

    [SerializeField] private float endEmitterSize = 1;

    [SerializeField] private ParticleSystem ps;


    public void StartAnim(float duration)
    {
        StartCoroutine(IAnimate(duration));
    }

    IEnumerator IAnimate(float duration)
    {
        var shape = ps.shape;
        float sizeDelta = startEmitterSize - endEmitterSize;
        shape.radius = startEmitterSize;
        
        ps.Play();
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
        ps.Clear();
        ps.Stop();
    }
}
