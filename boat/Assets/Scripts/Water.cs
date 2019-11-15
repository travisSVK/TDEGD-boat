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
    private int m_Length = 100;

    [SerializeField]
    private int m_Width = 10;

    [SerializeField]
    private int m_HorizontalChunks = 10;

    [SerializeField]
    private float m_VertexDensity;

    [SerializeField]
    private List<WaveProperties> m_Waves = new List<WaveProperties>();

    private Mesh m_Mesh = null;

    private MeshCollider m_MeshCollider = null;

    private List<Vector3> m_Vertices = new List<Vector3>();

    private List<int> m_Indices = new List<int>();

    private float m_ElapsedTime = 0.0f;

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

    private void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int current = m_Vertices.Count;

        m_Vertices.Add(p0);
        m_Vertices.Add(p1);
        m_Vertices.Add(p3);

        m_Vertices.Add(p1);
        m_Vertices.Add(p2);
        m_Vertices.Add(p3);

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
        m_ElapsedTime = Time.realtimeSinceStartup;

        for (int i = 0; i < m_Vertices.Count; ++i)
        {
            if (m_Vertices[i].y > -5.0f)
            {
                m_Vertices[i] = new Vector3(m_Vertices[i].x, GetHeight(m_Vertices[i].x, m_Vertices[i].z), m_Vertices[i].z);
            }
        }

        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateTangents();
        m_Mesh.RecalculateBounds();

        m_MeshCollider.sharedMesh = m_Mesh;
    }

    private void FixedUpdate()
    {
        m_ElapsedTime = Time.realtimeSinceStartup;
    }
}
