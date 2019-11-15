using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEdgeMesh : MonoBehaviour
{
    private Water m_Water = null;

    private MeshGenerator m_MeshGenerator = null;

    private bool m_isInitialized = false;

    public bool Initialize(Water water)
    {
        m_Water = water;
        if (!m_Water)
        {
            Debug.Log("Cannot provide null value to water.");
            return false;
        }

        m_MeshGenerator = GetComponent(typeof(MeshGenerator)) as MeshGenerator;
        if (!m_MeshGenerator)
        {
            m_MeshGenerator = gameObject.AddComponent(typeof(MeshGenerator)) as MeshGenerator;
        }

        for (int x = 0; x < m_Water.chunkLength; ++x)
        {
            m_MeshGenerator.AddQuad(
                new Vector3(x * m_Water.vertexDensity, -10.0f, 0.0f),
                new Vector3(x * m_Water.vertexDensity, 0.0f, 0.0f),
                new Vector3((x + 1.0f) * m_Water.vertexDensity, 0.0f, 0.0f),
                new Vector3((x + 1.0f) * m_Water.vertexDensity, -10.0f, 0.0f)
            );
        }

        m_isInitialized = true;
        return true;
    }

    private void Update()
    {
        if (!m_isInitialized)
        {
            enabled = false;
            Debug.LogError("Trying to update an non-initialized WaterEdgeMesh.");
            return;
        }

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Vector3 position = transform.position;
        for (int i = 0; i < m_MeshGenerator.Count(); ++i)
        {
            Vector3 vertex = m_MeshGenerator.GetVertex(i);
            if (vertex.y > -5.0f)
            {
                vertex = new Vector3(vertex.x, m_Water.GetHeight(position.x + vertex.x, position.z + vertex.z), vertex.z);
            }
            m_MeshGenerator.SetVertex(i, vertex);
        }

        m_MeshGenerator.Refresh();
    }
}
