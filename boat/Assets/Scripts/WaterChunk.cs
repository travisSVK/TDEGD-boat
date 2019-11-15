using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    private Water m_Water = null;

    private WaterEdgeMesh m_WaterEdgeMesh = null;

    private WaterSurfaceMesh m_WaterSurfaceMesh = null;

    private bool m_isInitialized = false;

    public bool Initialize(Water water)
    {
        m_Water = water;
        if (!m_Water)
        {
            Debug.Log("Cannot provide null value to water.");
            return false;
        }

        GameObject waterEdgeObject = new GameObject("WaterEdge");
        waterEdgeObject.transform.parent = transform;
        waterEdgeObject.transform.position = transform.position;
        m_WaterEdgeMesh = waterEdgeObject.AddComponent(typeof(WaterEdgeMesh)) as WaterEdgeMesh;
        m_WaterEdgeMesh.Initialize(m_Water);

        GameObject waterSurfaceObject = new GameObject("WaterSurface");
        waterSurfaceObject.transform.parent = transform;
        waterSurfaceObject.transform.position = transform.position;
        m_WaterSurfaceMesh = waterSurfaceObject.AddComponent(typeof(WaterSurfaceMesh)) as WaterSurfaceMesh;
        m_WaterSurfaceMesh.Initialize(m_Water);

        m_isInitialized = true;
        return true;
    }
}
