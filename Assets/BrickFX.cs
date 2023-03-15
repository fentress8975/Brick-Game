using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_ParticleSystem;

    internal void Play()
    {
        transform.SetParent(null);
        m_ParticleSystem.Play();
        WaitForParticlesEnds(m_ParticleSystem.main.duration);
    }

    private IEnumerator WaitForParticlesEnds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
