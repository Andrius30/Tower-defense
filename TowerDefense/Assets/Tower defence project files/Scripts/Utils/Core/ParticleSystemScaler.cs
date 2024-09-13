using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode()]
public class ParticleSystemScaler : MonoBehaviour
{
    [SerializeField] ParticleSystem m_System;
    ParticleSystem.Particle[] m_Particles;
    ParticleSystemRenderer systemRenderer;
    public float m_Size = 1.0f;
    public float m_StartSpeed = 1.0f;

    private void LateUpdate()
    {
        InitializeIfNeeded();

        SetSizeOverLifeTimeSize();
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = m_System.GetParticles(m_Particles);

        float currentScale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3.0f;
        var main = m_System.main;

        main.startSpeed = m_StartSpeed * currentScale;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            m_Particles[i].size = currentScale;
        }

        m_System.SetParticles(m_Particles, numParticlesAlive);

    }

    void InitializeIfNeeded()
    {
        if (m_System == null)
        {
            m_System = GetComponentInChildren<ParticleSystem>();
        }

        if (m_Particles == null || m_Particles.Length < m_System.maxParticles)
        {
            systemRenderer = m_System.GetComponent<ParticleSystemRenderer>();
            m_Particles = new ParticleSystem.Particle[m_System.maxParticles];
        }
    }
    void SetSizeOverLifeTimeSize()
    {
        var sizeOverLifeTime = m_System.sizeOverLifetime;
        sizeOverLifeTime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        var key = (m_Size / 2) * 0.1f;
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(1f, key);
        sizeOverLifeTime.size = new ParticleSystem.MinMaxCurve(1.5f, curve);
    }
}
