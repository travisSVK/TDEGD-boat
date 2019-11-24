using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaweedMesh : MonoBehaviour
{
    [SerializeField] private Material m_SeaweedMaterial = null;
    [SerializeField] private List<Mesh> m_SeaweedMeshes = new List<Mesh>();

    private MeshGenerator m_MeshGenerator = null;
    private Water m_Water = null;

    private void AddMesh(Vector3 position, float scale)
    {

        int r = Random.Range(0, m_SeaweedMeshes.Count);
        for (int i = 0; i < m_SeaweedMeshes[0].triangles.Length; i += 3)
        {
            m_MeshGenerator.AddTriangle(
                position + (m_SeaweedMeshes[r].vertices[m_SeaweedMeshes[r].triangles[i + 0]] * scale),
                position + (m_SeaweedMeshes[r].vertices[m_SeaweedMeshes[r].triangles[i + 1]] * scale),
                position + (m_SeaweedMeshes[r].vertices[m_SeaweedMeshes[r].triangles[i + 2]] * scale),
                m_SeaweedMeshes[r].uv[m_SeaweedMeshes[r].triangles[i + 0]],
                m_SeaweedMeshes[r].uv[m_SeaweedMeshes[r].triangles[i + 1]],
                m_SeaweedMeshes[r].uv[m_SeaweedMeshes[r].triangles[i + 2]]);
        }
    }

    private void AddSeaweedChunk(Vector3 position)
    {
        List<Vector3> scatterPoints = new List<Vector3>();
        for (int x = -10; x <= 10; ++x)
        {
            for (int z = -10; z <= 10; ++z)
            {
                float x0 = (x / 20.0f) + position.x;
                float z0 = (z / 20.0f) + position.z;
                scatterPoints.Add(new Vector3(x0, 0.0f, z0));
            }
        }

        int chunkSize = Random.Range(5, 10);
        for (int i = 0; i < chunkSize; ++i)
        {
            int r = Random.Range(0, scatterPoints.Count);
            Vector3 newPostion = scatterPoints[r];
            scatterPoints.RemoveAt(r);
            newPostion.y = m_Water.GetFloorDepth(newPostion.x, newPostion.z);
            if (newPostion.z > 0.1f)
            {
                AddMesh(newPostion, Random.Range(0.8f, 3.2f));
            }
        }
    }

    private void Awake()
    {
        m_Water = FindObjectOfType(typeof(Water)) as Water;
        m_MeshGenerator = GetComponent(typeof(MeshGenerator)) as MeshGenerator;

        if (!m_MeshGenerator)
        {
            m_MeshGenerator = gameObject.AddComponent(typeof(MeshGenerator)) as MeshGenerator;
        }

        m_MeshGenerator.material = m_SeaweedMaterial;

        m_MeshGenerator.Clear();

        for (int x = 0; x < 20; ++x)
        {
            float x0 = x + 0.5f + transform.position.x;
            for (int z = 0; z < 10; ++z)
            {
                float z0 = z + 0.5f;

                if (Mathf.PerlinNoise((x0 / 20.0f) * 6.0f, (z0 / 10.0f) * 6.0f) > 0.7f)
                {
                    Vector3 position = new Vector3(x0, m_Water.GetFloorDepth(x0, z0), z0);
                    AddSeaweedChunk(position);
                }
            }
        }

        m_MeshGenerator.Refresh();
    }
}
