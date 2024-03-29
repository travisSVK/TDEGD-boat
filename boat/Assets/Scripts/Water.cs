﻿using System;
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

    [SerializeField] private float m_ShoreThreshold = 200.0f;
    [SerializeField] private int m_ChunkLength = 20;
    [SerializeField] private int m_Width = 10;
    [SerializeField] private int m_ChunkCount = 10;
    [SerializeField] private int m_Subdivisions = 1;
    [SerializeField] private Material m_FloorEdgeMaterial = null;
    [SerializeField] private Material m_FloorSurfaceMaterial = null;
    [SerializeField] private Material m_WaterEdgeMaterial = null;
    [SerializeField] private Material m_WaterSurfaceMaterial = null;
    [SerializeField] private List<WaveProperties> m_Waves = new List<WaveProperties>();

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

    public float GetWaterHeight(float x, float z)
    {
        float waveFactor = 0.0f;
        float noiseFactor = 0.0f;
        float shoreFactor = Mathf.SmoothStep(0.15f, 1.0f, Mathf.Min(1.0f, x / m_ShoreThreshold));

        for (int i = 0; i < m_Waves.Count; ++i)
        {
            float elapsedTime = m_ElapsedTime * m_Waves[i].waveVelocity;
            waveFactor += Mathf.Sin(elapsedTime + x * m_Waves[i].waveFrequency) * m_Waves[i].waveAmplitude * m_Waves[i].influence * shoreFactor;
            noiseFactor += Mathf.PerlinNoise((x + elapsedTime) * m_Waves[i].noiseFrequency, z * m_Waves[i].noiseFrequency) * m_Waves[i].noiseStrength * m_Waves[i].influence;
        }

        return waveFactor + noiseFactor;
    }

    public float GetFloorDepth(float x, float z)
    {
        return  (Mathf.PerlinNoise(x * 0.17f, z * 0.17f) * 5.0f) *
            Mathf.SmoothStep(0.2f, 1.0f, Mathf.Min(1.0f, (x * 2.0f) / m_ShoreThreshold)) - 
            Mathf.SmoothStep(3.0f, 10.0f, Mathf.Min(1.0f, (x * 2.0f) / m_ShoreThreshold));
    }

    public bool IsUnderwater(Vector3 point)
    {
        return GetWaterHeight(point.x, point.z) > point.y;
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
