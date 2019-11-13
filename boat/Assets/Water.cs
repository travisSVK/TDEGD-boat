using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField]
    private float m_VertexDensity = 0.1f;

    [SerializeField]
    private float m_WaveAmplitude = 1.0f;

    [SerializeField]
    private float m_WaveCurrent = 0.5f;

    [SerializeField]
    private int m_Length = 100;

    private Mesh m_Mesh = null;

    private MeshCollider m_MeshCollider = null;

    private List<Vector3> m_Vertices = new List<Vector3>();

    private List<int> m_Indices = new List<int>();

    private float elapsedTime = 0.0f;

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
            Debug.Log(m_VertexDensity);

            AddQuad(
                new Vector3(x * m_VertexDensity, 0.0f, -2.0f),
                new Vector3(x * m_VertexDensity, 0.0f, 2.0f),
                new Vector3((x + 1.0f) * m_VertexDensity, 0.0f, 2.0f),
                new Vector3((x + 1.0f) * m_VertexDensity, 0.0f, -2.0f)
            );

            AddQuad(
                new Vector3(x * m_VertexDensity, -10.0f, -2.0f),
                new Vector3(x * m_VertexDensity, 0.0f, -2.0f),
                new Vector3((x + 1.0f) * m_VertexDensity, 0.0f, -2.0f),
                new Vector3((x + 1.0f) * m_VertexDensity, -10.0f, -2.0f)
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
        Vector3[] transformedVertices = new Vector3[m_Vertices.Count];
        elapsedTime += Time.deltaTime * m_WaveCurrent;

        for (int i = 0; i < m_Vertices.Count; ++i)
        {
            if (m_Vertices[i].y > -0.1f)
            {
                float yValue = Mathf.Sin(elapsedTime + m_Vertices[i].x);
                transformedVertices[i] = new Vector3(m_Vertices[i].x, yValue * m_WaveAmplitude, m_Vertices[i].z);
            }
            else
            {
                transformedVertices[i] = m_Vertices[i];
            }
        }

        m_Mesh.vertices = transformedVertices;
        m_Mesh.triangles = m_Indices.ToArray();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateTangents();
        m_Mesh.RecalculateBounds();

        m_MeshCollider.sharedMesh = m_Mesh;
    }
}
