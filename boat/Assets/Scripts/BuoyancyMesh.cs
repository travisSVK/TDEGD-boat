﻿using UnityEngine;
using System.Collections.Generic;

public class BuoyancyMesh : MonoBehaviour
{
    [SerializeField] private Mesh m_Mesh = null;
    private Rigidbody m_RigidBody;

    private Vector3[] m_BoatVertices = null;
    private List<Triangle> m_AbovewaterTriangles = new List<Triangle>();
    private List<Triangle> m_UnderwaterTriangles = new List<Triangle>();
    private List<Vector3> m_IntersectionVertices = new List<Vector3>();
    private Vector3[] m_BoatVerticesGlobal = null;
    private int[] m_BoatIndices = null;
    private float[] m_WaterDistances = null;
    private Water m_Water = null;

    public Mesh mesh
    {
        get
        {
            return m_Mesh;
        }
    }

    public List<Triangle> underwaterTriangles
    {
        get 
        {
            return m_UnderwaterTriangles;
        }
    }

    public List<Triangle> abovewaterTriangles
    {
        get
        {
            return m_AbovewaterTriangles;
        }
    }

    public List<Vector3> intersectionVertices
    {
        get
        {
            return m_IntersectionVertices;
        }
    }

    private class VertexData
    {
        // The distance to water from this vertex.
        public float distance;

        // An index so we can form clockwise triangles.
        public int index;

        // The global Vector3 position of the vertex.
        public Vector3 globalVertexPos;
    }

    private void Start()
    {
        m_BoatVertices = m_Mesh.vertices;
        m_BoatIndices = m_Mesh.triangles;
        m_BoatVerticesGlobal = new Vector3[m_BoatVertices.Length];
        m_WaterDistances = new float[m_BoatVertices.Length];
        m_Water = FindObjectOfType(typeof(Water)) as Water;
        m_RigidBody = GetComponent(typeof(Rigidbody)) as Rigidbody;
    }

    public float GetLength()
    {
        // length is in the x direction
        return m_Mesh.bounds.size.x;
    }

    /*
     * Keep the infor about mesh triangles that reside under water
     * Follows algorithm from this article:
     * https://www.gamasutra.com/view/news/237528/Water_interaction_model_for_boats_in_video_games.php
     */
    private void FixedUpdate()
    {
        m_AbovewaterTriangles.Clear();
        m_UnderwaterTriangles.Clear();
        m_IntersectionVertices.Clear();
        PrepareForGeneration();
        List<VertexData> vertexData = new List<VertexData>();
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());

        // Loop throough the triangles.
        for (int i = 0; i < m_BoatIndices.Length; i += 3)
        {
            for (int x = 0; x < 3; x++)
            {
                vertexData[x].distance = m_WaterDistances[m_BoatIndices[i + x]];
                vertexData[x].index = x;
                vertexData[x].globalVertexPos = m_BoatVerticesGlobal[m_BoatIndices[i + x]];
            }

            if (vertexData[0].distance > 0.0f && vertexData[1].distance > 0.0f && vertexData[2].distance > 0.0f)
            {
                m_AbovewaterTriangles.Add(
                    new Triangle(vertexData[0].globalVertexPos, vertexData[1].globalVertexPos, vertexData[2].globalVertexPos, m_RigidBody, m_Water));
                continue;
            }

            if (vertexData[0].distance < 0.0f && vertexData[1].distance < 0.0f && vertexData[2].distance < 0.0f)
            {
                m_UnderwaterTriangles.Add(
                    new Triangle(vertexData[0].globalVertexPos, vertexData[1].globalVertexPos, vertexData[2].globalVertexPos, m_RigidBody, m_Water));
                continue;
            }

            int aboveWater = GetNumberAboveWater(vertexData);

            if (aboveWater == 1)
            {
                VertexAbove(vertexData);
            }
            else if (aboveWater == 2)
            {
                TwoVerticesAbove(vertexData);
            }
        }
    }

    /*
     * Add triangles with one vertex above water surface.
     */
    private void VertexAbove(List<VertexData> vertexData)
    {
        //H is always at position 0 (see GetNumberAboveWater method description)
        Vector3 H = vertexData[0].globalVertexPos;

        //Left of H is M (see GetNumberAboveWater method description)
        //Right of H is L (see GetNumberAboveWater method description)

        //Find the index of M
        int M_index = vertexData[0].index - 1;
        if (M_index < 0)
        {
            M_index = 2;
        }

        //We also need the heights to water
        float h_H = vertexData[0].distance;
        float h_M;
        float h_L;

        Vector3 M = Vector3.zero;
        Vector3 L = Vector3.zero;

        // find M vertex based on its index
        if (vertexData[1].index == M_index)
        {
            M = vertexData[1].globalVertexPos;
            L = vertexData[2].globalVertexPos;
            h_M = vertexData[1].distance;
            h_L = vertexData[2].distance;
        }
        else
        {
            M = vertexData[2].globalVertexPos;
            L = vertexData[1].globalVertexPos;
            h_M = vertexData[2].distance;
            h_L = vertexData[1].distance;
        }


        // Cut the triangle and form two new

        // get point I_M laying on edge MH
        Vector3 MH = H - M;
        float t_M = -h_M / (h_H - h_M); // parametrized amount as a ratio of edge part under water  
        Vector3 MI_M = t_M * MH;
        Vector3 I_M = MI_M + M; // add the parametrized amount to the vertex coordinate

        // get point I_L laying on edge LH 
        Vector3 LH = H - L;
        float t_L = -h_L / (h_H - h_L);
        Vector3 LI_L = t_L * LH;
        Vector3 I_L = LI_L + L;

        // add 2 triangles
        m_UnderwaterTriangles.Add(new Triangle(M, I_M, I_L, m_RigidBody, m_Water));
        m_UnderwaterTriangles.Add(new Triangle(M, I_L, L, m_RigidBody, m_Water));
        m_AbovewaterTriangles.Add(new Triangle(I_M, H, I_L, m_RigidBody, m_Water));

        m_IntersectionVertices.Add(I_M);
        m_IntersectionVertices.Add(I_L);
    }

    /*
     * Add triangles with two vertices above water surface.
     */
    private void TwoVerticesAbove(List<VertexData> vertexData)
    {
        // L is always the last 
        Vector3 L = vertexData[2].globalVertexPos;

        //H and M are above the water
        //H is after the vertex that's below water, which is L

        //Find the index of H
        int H_index = vertexData[2].index + 1;
        if (H_index > 2)
        {
            H_index = 0;
        }

        //We also need the heights to water
        float h_L = vertexData[2].distance;
        float h_H = 0f;
        float h_M = 0f;

        Vector3 H = Vector3.zero;
        Vector3 M = Vector3.zero;

        //This means that H is at position 1 in the list
        if (vertexData[1].index == H_index)
        {
            H = vertexData[1].globalVertexPos;
            M = vertexData[0].globalVertexPos;

            h_H = vertexData[1].distance;
            h_M = vertexData[0].distance;
        }
        else
        {
            H = vertexData[0].globalVertexPos;
            M = vertexData[1].globalVertexPos;

            h_H = vertexData[0].distance;
            h_M = vertexData[1].distance;
        }


        // Cut the triangle and form one new

        // get point J_M laying on edge LM
        Vector3 LM = M - L;
        float t_M = -h_L / (h_M - h_L); // parametrized amount as a ratio of edge part under water   
        Vector3 LJ_M = t_M * LM;
        Vector3 J_M = LJ_M + L; // add the parametrized amount to the vertex coordinate


        // get point J_H laying on edge LH
        Vector3 LH = H - L;
        float t_H = -h_L / (h_H - h_L);
        Vector3 LJ_H = t_H * LH;
        Vector3 J_H = LJ_H + L;

        //1 triangle below the water
        m_UnderwaterTriangles.Add(new Triangle(L, J_H, J_M, m_RigidBody, m_Water));
        m_AbovewaterTriangles.Add(new Triangle(J_H, H, J_M, m_RigidBody, m_Water));
        m_AbovewaterTriangles.Add(new Triangle(J_M, H, M, m_RigidBody, m_Water));

        m_IntersectionVertices.Add(J_H);
        m_IntersectionVertices.Add(J_M);
    }

    /*
    * Get number of vertices above water. At the same time, 
    * sort the vertices by the highest (positive) distance from water
    */
    private int GetNumberAboveWater(List<VertexData> vertexData)
    {
        int numberAboveWater = 0;
        VertexData min = vertexData[0];
        VertexData mid = vertexData[0];
        VertexData max = vertexData[0];
        foreach (VertexData vd in vertexData)
        {
            if (vd.distance < min.distance)
            {
                if (min != mid)
                {
                    max = mid;
                    mid = min;
                }
                min = vd;
            }
            else if (vd.distance > max.distance)
            {
                if (max != mid)
                {
                    min = mid;
                    mid = max;
                }
                max = vd;
            }
            else
            {
                mid = vd;
            }
            if (vd.distance > 0.0f)
            {
                numberAboveWater++;
            }
        }

        vertexData[0] = max;
        vertexData[1] = mid;
        vertexData[2] = min;
        return numberAboveWater;
    }

    /*
     * Generate global positions and water surface distances beforehand, because
     * some vertices are shared (decreasing number of computations)
     */
    private void PrepareForGeneration()
    {
        for (int i = 0; i < m_BoatVertices.Length; i++)
        {
            Vector3 globalPosition = transform.TransformPoint(m_BoatVertices[i]);
            m_BoatVerticesGlobal[i] = globalPosition;
            float waterHeight = m_Water.GetWaterHeight(globalPosition.x, globalPosition.z);
            // positive -> above water and vice versa
            m_WaterDistances[i] = globalPosition.y - waterHeight;
        }
    }
}
