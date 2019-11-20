using UnityEngine;

public class FloorEdgeMesh : MonoBehaviour
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

        m_MeshGenerator.material = m_Water.floorEdgeMaterial;

        UpdateMesh();

        return true;
    }

    private void UpdateMesh()
    {
        m_MeshGenerator.Clear();

        Vector3 position = transform.position;
        for (int x = 0; x < m_Water.chunkLength; ++x)
        {
            float minX = x / (float)m_Water.subdivisions;
            float maxX = (x + 1) / (float)m_Water.subdivisions;

            Vector3 p0 = new Vector3(minX,-20.0f, 0.0f);
            Vector3 p1 = new Vector3(minX, m_Water.GetFloorDepth(position.x + minX, 0.0f), 0.0f);
            Vector3 p2 = new Vector3(maxX, m_Water.GetFloorDepth(position.x + maxX, 0.0f), 0.0f);
            Vector3 p3 = new Vector3(maxX, -20.0f, 0.0f);

            m_MeshGenerator.AddQuad(p0, p1, p2, p3);
        }

        m_MeshGenerator.Refresh();
    }
}
