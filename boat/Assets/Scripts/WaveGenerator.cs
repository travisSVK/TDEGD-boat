using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    static private List<WaveGenerator> m_WaveGenerators = new List<WaveGenerator>();

    static public List<WaveGenerator> waveGenerators
    {
        get
        {
            return m_WaveGenerators;
        }
    }

    public float GetInfulence(float x, float z, float elapsedTime)
    {
        return 0.0f;
    }

    private void OnEnable()
    {
        m_WaveGenerators.Add(this);
    }

    private void OnDisable()
    {
        m_WaveGenerators.Remove(this);        
    }
}
