using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarFX : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_ThrustersFW;
    [SerializeField] private List<ParticleSystem> m_ThrustersLeft;
    [SerializeField] private List<ParticleSystem> m_ThrustersRight;
    [SerializeField] private List<ParticleSystem> m_ThrustersDown;

    private void Start()
    {
        StopAllEffects();
    }

    public void ReadMovementDirection(Direction direction)
    {
        StopAllEffects();
        switch (direction)
        {
            case Direction.Left:
                foreach (ParticleSystem p in m_ThrustersLeft)
                {
                    p.Play();
                }
                break;
            case Direction.Right:
                foreach (ParticleSystem p in m_ThrustersRight)
                {
                    p.Play();
                }
                break;
            case Direction.None:
                break;
        }
    }

    private void StopAllEffects()
    {
        foreach (ParticleSystem p in m_ThrustersFW)
        {
            p.Stop();
        }
        foreach (ParticleSystem p in m_ThrustersLeft)
        {
            p.Stop();
        }
        foreach (ParticleSystem p in m_ThrustersRight)
        {
            p.Stop();
        }
        foreach (ParticleSystem p in m_ThrustersDown)
        {
            p.Stop();
        }
    }
}
