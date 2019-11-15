﻿using System.Collections;
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

        m_isInitialized = true;
        return true;
    }
}
