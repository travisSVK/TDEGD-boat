using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotator : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem = null;
    ParticleSystem.Particle[] m_Particles;


    private void LateUpdate()
    {
        InitializeIfNeeded();
        int count = m_ParticleSystem.GetParticles(m_Particles);
        for (int i = 0; i < count; i++)
        {
            if (((m_Particles[i].startLifetime - m_Particles[i].remainingLifetime) >= (m_Particles[i].startLifetime / 3.0f)) &&
                ((m_Particles[i].startLifetime - m_Particles[i].remainingLifetime) <= (m_Particles[i].startLifetime / 3.0f * 2.0f)))
            {
                float scaleFactor = 2.0f * Mathf.PI / (m_Particles[i].startLifetime / 3.0f);
                float cos = Mathf.Cos(scaleFactor * ((m_Particles[i].startLifetime - m_Particles[i].remainingLifetime) - (m_Particles[i].startLifetime / 3.0f)));
                float sin = Mathf.Sin(scaleFactor * ((m_Particles[i].startLifetime - m_Particles[i].remainingLifetime) - (m_Particles[i].startLifetime / 3.0f)));
                //Debug.Log("cos: " + cos + " sin: " + sin);
                m_Particles[i].velocity = new Vector3(-9.0f * cos, 9.0f * sin, 0.0f);
            }
            else
            {
                m_Particles[i].velocity = new Vector3(-7.0f, 0.0f, 0.0f);
            }
        }
        m_ParticleSystem.SetParticles(m_Particles, count);
    }

    private void InitializeIfNeeded()
    {
        if (!m_ParticleSystem)
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        if (m_Particles == null || m_Particles.Length < m_ParticleSystem.main.maxParticles)
        {
            m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
        }
    }
}
