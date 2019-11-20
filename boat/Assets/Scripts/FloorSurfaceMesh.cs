using UnityEngine;

public class FloorSurfaceMesh : MonoBehaviour
{
    private Water m_Water;
    private MeshGenerator m_MeshGenerator = null;

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

        m_MeshGenerator.material = m_Water.floorSurfaceMaterial;
        UpdateMesh();

        return true;
    }

    private void UpdateMesh()
    {
        m_MeshGenerator.Clear();

        Vector3 position = transform.position;
        for (int x = 0; x < m_Water.chunkLength; ++x)
        {
            for (int z = 0; z < m_Water.width; ++z)
            {
                float minX = x / (float)m_Water.subdivisions;
                float maxX = (x + 1) / (float)m_Water.subdivisions;
                float minZ = z / (float)m_Water.subdivisions;
                float maxZ = (z + 1) / (float)m_Water.subdivisions;

                Vector3 p0 = new Vector3(minX, m_Water.GetFloorDepth(position.x + minX, position.z + minZ), minZ);
                Vector3 p1 = new Vector3(minX, m_Water.GetFloorDepth(position.x + minX, position.z + maxZ), maxZ);
                Vector3 p2 = new Vector3(maxX, m_Water.GetFloorDepth(position.x + maxX, position.z + maxZ), maxZ);
                Vector3 p3 = new Vector3(maxX, m_Water.GetFloorDepth(position.x + maxX, position.z + minZ), minZ);

                m_MeshGenerator.AddQuad(p0, p1, p2, p3);
            }
        }

        m_MeshGenerator.Refresh();
    }
}
