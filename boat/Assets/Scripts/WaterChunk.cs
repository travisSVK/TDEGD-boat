using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    private Water m_Water = null;
    private FloorEdgeMesh m_FloorEdgeMesh = null;
    private FloorSurfaceMesh m_FloorSurfaceMesh = null;
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

        GameObject floorEdgeObject = new GameObject("FloorEdge");
        floorEdgeObject.transform.parent = transform;
        floorEdgeObject.transform.position = transform.position;
        m_FloorEdgeMesh = floorEdgeObject.AddComponent(typeof(FloorEdgeMesh)) as FloorEdgeMesh;
        if (!m_FloorEdgeMesh.Initialize(m_Water))
        {
            return false;
        }

        GameObject floorSurfaceObject = new GameObject("FloorSurface");
        floorSurfaceObject.transform.parent = transform;
        floorSurfaceObject.transform.position = transform.position;
        m_FloorSurfaceMesh = floorSurfaceObject.AddComponent(typeof(FloorSurfaceMesh)) as FloorSurfaceMesh;
        if (!m_FloorSurfaceMesh.Initialize(m_Water))
        {
            return false;
        }

        GameObject waterEdgeObject = new GameObject("WaterEdge");
        waterEdgeObject.transform.parent = transform;
        waterEdgeObject.transform.position = transform.position;
        m_WaterEdgeMesh = waterEdgeObject.AddComponent(typeof(WaterEdgeMesh)) as WaterEdgeMesh;
        if (!m_WaterEdgeMesh.Initialize(m_Water))
        {
            return false;
        }

        GameObject waterSurfaceObject = new GameObject("WaterSurface");
        waterSurfaceObject.transform.parent = transform;
        waterSurfaceObject.transform.position = transform.position;
        m_WaterSurfaceMesh = waterSurfaceObject.AddComponent(typeof(WaterSurfaceMesh)) as WaterSurfaceMesh;
        if (!m_WaterSurfaceMesh.Initialize(m_Water))
        {
            return false;
        }

        m_isInitialized = true;
        return true;
    }

    private void Update()
    {
        if (!m_isInitialized)
        {
            enabled = false;
            Debug.LogError("Trying to update an non-initialized WaterSurfaceMesh.");
            return;
        }

        Camera camera = Camera.main;


    }
}
