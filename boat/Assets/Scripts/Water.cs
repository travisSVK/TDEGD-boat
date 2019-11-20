using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Serializable]
    public struct WaveProperties
    {
        public float waveAmplitude;
        public float waveVelocity;
        public float waveFrequency;
        public float noiseFrequency;
        public float noiseStrength;
        public float influence;
    }

    [SerializeField]
    private int m_ChunkLength = 20;

    [SerializeField]
    private int m_Width = 10;

    [SerializeField]
    private int m_ChunkCount = 10;

    [SerializeField]
    private int m_Subdivisions = 1;

    [SerializeField]
    private Material m_FloorEdgeMaterial = null;

    [SerializeField]
    private Material m_FloorSurfaceMaterial = null;

    [SerializeField]
    private Material m_WaterEdgeMaterial = null;

    [SerializeField]
    private Material m_WaterSurfaceMaterial = null;

    [SerializeField]
    private List<WaveProperties> m_Waves = new List<WaveProperties>();

    private List<WaterChunk> m_WaterChunks = new List<WaterChunk>();

    private float m_ElapsedTime = 0.0f;

    public int chunkLength
    {
        get
        {
            return m_ChunkLength;
        }
    }

    public int width
    {
        get
        {
            return m_Width;
        }
    }

    public int subdivisions
    {
        get
        {
            return m_Subdivisions;
        }
    }

    public Material floorEdgeMaterial
    {
        get
        {
            return m_FloorEdgeMaterial;
        }
    }

    public Material floorSurfaceMaterial
    {
        get
        {
            return m_FloorSurfaceMaterial;
        }
    }

    public Material waterEdgeMaterial
    {
        get
        {
            return m_WaterEdgeMaterial;
        }
    }

    public Material waterSurfaceMaterial
    {
        get
        {
            return m_WaterSurfaceMaterial;
        }
    }

    public float GetHeight(float x, float z)
    {
        float height = 0.0f;
        for (int i = 0; i < m_Waves.Count; ++i)
        {
            float elapsedTime = m_ElapsedTime * m_Waves[i].waveVelocity;
            float waveFactor = Mathf.Sin(elapsedTime + x * m_Waves[i].waveFrequency) * m_Waves[i].waveAmplitude;
            float noiseFactor = Mathf.PerlinNoise((x + elapsedTime) * m_Waves[i].noiseFrequency, z * m_Waves[i].noiseFrequency) * m_Waves[i].noiseStrength;
            height += (waveFactor + noiseFactor) * m_Waves[i].influence;
        }
        return height;
    }

    private void Awake()
    {
        // To avoid division with zero.
        if (m_Subdivisions <= 0)
        {
            m_Subdivisions = 1;
        }

        for (int i = 0; i < m_ChunkCount; ++i)
        {
            GameObject go = new GameObject("WaterChunk_" + i);
            go.transform.parent = transform;
            go.transform.position = new Vector3(
                transform.position.x + m_ChunkLength * i,
                transform.position.y, transform.position.z
            );
            WaterChunk waterChunk = go.AddComponent(typeof(WaterChunk)) as WaterChunk;
            waterChunk.Initialize(this);
            m_WaterChunks.Add(waterChunk);
        }
    }

    private void Update()
    {
        m_ElapsedTime = Time.realtimeSinceStartup;
    }

    private void FixedUpdate()
    {
        m_ElapsedTime = Time.realtimeSinceStartup;
    }
}
