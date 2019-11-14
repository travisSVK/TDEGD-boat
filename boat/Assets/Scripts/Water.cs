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

    [SerializeField]
    private  int m_Length = 100;
    
    [SerializeField]
    private int m_Width = 10;
    
    [SerializeField]
    private float m_VertexDensity;

    [SerializeField]
    private List<WaveProperties> m_Waves = new List<WaveProperties>();

    private Mesh m_Mesh = null;

    private MeshCollider m_MeshCollider = null;

    private List<Vector3> m_Vertices = new List<Vector3>();

    private List<Color> m_Color = new List<Color>();

    private List<Vector3> m_TranslatedVertices = new List<Vector3>();

    private List<int> m_Indices = new List<int>();

    private void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int current = m_Vertices.Count;

        m_Vertices.Add(p0);
        m_Vertices.Add(p1);
        m_Vertices.Add(p3);

        m_Vertices.Add(p1);
        m_Vertices.Add(p2);
        m_Vertices.Add(p3);

        m_TranslatedVertices.Add(p0);
        m_TranslatedVertices.Add(p1);
        m_TranslatedVertices.Add(p3);

        m_TranslatedVertices.Add(p1);
        m_TranslatedVertices.Add(p2);
        m_TranslatedVertices.Add(p3);

        m_Color.Add(Color.blue);
        m_Color.Add(Color.blue);
        m_Color.Add(Color.blue);

        m_Color.Add(Color.blue);
        m_Color.Add(Color.blue);
        m_Color.Add(Color.blue);

        m_Indices.Add(current + 0);
        m_Indices.Add(current + 1);
        m_Indices.Add(current + 2);

        m_Indices.Add(current + 3);
        m_Indices.Add(current + 4);
        m_Indices.Add(current + 5);
    }

    private void Start()
    {
        m_Mesh = new Mesh();
        MeshFilter meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        m_MeshCollider = GetComponent(typeof(MeshCollider)) as MeshCollider;

        for (int x = 0; x < m_Length; ++x)
        {
            for (int z = 0; z < m_Width; ++z)
            {
                AddQuad(
                    new Vector3(x * m_VertexDensity, 0.0f, z * m_VertexDensity),
                    new Vector3(x * m_VertexDensity, 0.0f, (z + 1.0f) * m_VertexDensity),
                    new Vector3((x + 1.0f) * m_VertexDensity, 0.0f, (z + 1.0f) * m_VertexDensity),
                    new Vector3((x + 1.0f) * m_VertexDensity, 0.0f, z * m_VertexDensity)
                );
            }

            AddQuad(
                new Vector3(x * m_VertexDensity, -10.0f, 0.0f),
                new Vector3(x * m_VertexDensity, 0.0f, 0.0f),
                new Vector3((x + 1.0f) * m_VertexDensity, 0.0f, 0.0f),
                new Vector3((x + 1.0f) * m_VertexDensity, -10.0f, 0.0f)
            );
        }

        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.triangles = m_Indices.ToArray();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateTangents();
        m_Mesh.RecalculateBounds();

        meshFilter.mesh = m_Mesh;
        m_MeshCollider.sharedMesh = m_Mesh;
    }

    private void Update()
    {

        for (int i = 0; i < m_Vertices.Count; ++i)
        {
            if (m_Vertices[i].y > -0.1f)
            {
                m_TranslatedVertices[i] = m_Vertices[i];
                for (int j = 0; j < m_Waves.Count; ++j)
                {
                    float elapsedTime = Time.realtimeSinceStartup * m_Waves[j].waveVelocity;
                    float yValue = Mathf.Sin(elapsedTime + m_Vertices[i].x * m_Waves[j].waveFrequency);
                    yValue += Mathf.PerlinNoise((m_Vertices[i].x + elapsedTime) * m_Waves[j].noiseFrequency, m_Vertices[i].z * m_Waves[j].noiseFrequency) * m_Waves[j].noiseStrength;
                    m_TranslatedVertices[i] += new Vector3(m_Vertices[i].x, yValue * m_Waves[j].waveAmplitude, m_Vertices[i].z) * m_Waves[j].influence;
                }
            }
            else
            {
                m_TranslatedVertices[i] = m_Vertices[i];
            }
        }

        m_Mesh.vertices = m_TranslatedVertices.ToArray();
        m_Mesh.colors = m_Color.ToArray();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateTangents();
        m_Mesh.RecalculateBounds();

        m_MeshCollider.sharedMesh = m_Mesh;
    }
}
