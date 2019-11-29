using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoamMesh : MonoBehaviour
{
    [SerializeField] private Material m_FoamMaterial = null;
    [SerializeField] private float m_FoamSize = 0.5f;
    [SerializeField] private float distBetweenNewVertices = 0.5f;

    private Water m_Water = null;
    private BuoyancyMesh m_Bouyancymesh = null;
    private Mesh m_Mesh = null;

    private void Awake()
    {
        m_Bouyancymesh = GetComponent(typeof(BuoyancyMesh)) as BuoyancyMesh;

        m_Water = FindObjectOfType(typeof(Water)) as Water;

        GameObject go = new GameObject();
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        m_Mesh = new Mesh();

        MeshRenderer meshRenderer = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = m_FoamMaterial;

        MeshFilter meshFilter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        meshFilter.mesh = m_Mesh;
    }

    private void Update()
    {
        m_Mesh.Clear();

        if (m_Bouyancymesh && m_Bouyancymesh.intersectionVertices.Count >= 4)
        {
            GenerateFoamSkirt(m_Bouyancymesh.intersectionVertices);
        }
    }

    private List<Vector3> AddVertices(List<Vector3> sortedVertices)
    {
        List<Vector3> finalVertices = new List<Vector3>();

        for (int i = 0; i < sortedVertices.Count; i++)
        {
            int lastVertPos = i - 1;

            if (lastVertPos < 0)
            {
                lastVertPos = sortedVertices.Count - 1;
            }

            Vector3 lastVert = sortedVertices[lastVertPos];
            Vector3 thisVert = sortedVertices[i];

            float distance = Vector3.Magnitude(thisVert - lastVert);

            Vector3 dir = Vector3.Normalize((thisVert - lastVert));

            //How many new vertices can we fit between?
            int newVertices = Mathf.FloorToInt(distance / distBetweenNewVertices);

            //Add the new vertices
            finalVertices.Add(lastVert);

            for (int j = 1; j < newVertices; j++)
            {
                Vector3 newVert = lastVert + j * dir * distBetweenNewVertices;

                finalVertices.Add(newVert);
            }
        }

        finalVertices.Add(sortedVertices[sortedVertices.Count - 1]);
        for (int i = 0; i < finalVertices.Count; i++)
        {
            Vector3 v = finalVertices[i];
            v.y = m_Water.GetWaterHeight(v.x, v.z);
            v.y += 0.1f;
            finalVertices[i] = v;
        }

        return finalVertices;
    }

    private void CreateFoamMesh(List<Vector3> finalVertices)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Vector3 TL = finalVertices[finalVertices.Count - 1];
        Vector3 TR = finalVertices[0];
        Vector3 vecBetween = Vector3.Normalize(TR - TL);
        Vector3 normal = new Vector3(vecBetween.z, 0f, -vecBetween.x);
        Vector3 vecBetweenLeft = Vector3.Normalize(TL - finalVertices[finalVertices.Count - 2]);
        Vector3 normalLeft = new Vector3(vecBetweenLeft.z, 0f, -vecBetweenLeft.x);
        Vector3 averageNormalLeft = Vector3.Normalize((normalLeft + normal) * 0.5f);

        Vector3 BL = TL + averageNormalLeft * m_FoamSize;
        BL.y = m_Water.GetWaterHeight(BL.x, BL.z);

        Vector3 TL_local = transform.InverseTransformPoint(TL);
        Vector3 BL_local = transform.InverseTransformPoint(BL);

        vertices.Add(TL_local);
        vertices.Add(BL_local);

        for (int i = 0; i < finalVertices.Count; i++)
        {
            // Right side.
            int rightPos = i + 1;
            if (rightPos > finalVertices.Count - 1)
            {
                rightPos = 0;
            }

            Vector3 vecBetweenRight = Vector3.Normalize(finalVertices[rightPos] - TR);
            Vector3 normalRight = new Vector3(vecBetweenRight.z, 0f, -vecBetweenRight.x);
            Vector3 averageNormalRight = Vector3.Normalize((normalRight + normal) * 0.5f);
            Vector3 BR = TR + averageNormalRight * m_FoamSize;

            BR.y = m_Water.GetWaterHeight(BR.x, BR.z);

            Vector3 TR_local = transform.InverseTransformPoint(TR);
            Vector3 BR_local = transform.InverseTransformPoint(BR);

            vertices.Add(TR_local);
            vertices.Add(BR_local);

            triangles.Add(vertices.Count - 4);
            triangles.Add(vertices.Count - 1);
            triangles.Add(vertices.Count - 3);

            triangles.Add(vertices.Count - 4);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            normal = normalRight;
            TR = finalVertices[rightPos];
        }

        m_Mesh.vertices = vertices.ToArray();
        m_Mesh.triangles = triangles.ToArray();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateBounds();
    }

    private List<Vector3> CleanVertices(List<Vector3> intersectionVertices)
    {
        List<Vector3> cleanedVertices = new List<Vector3>();
        for (int i = 0; i < intersectionVertices.Count; i++)
        {
            bool hasFoundNearbyVertice = false;

            for (int j = 0; j < cleanedVertices.Count; j++)
            {
                //Is the list already including a vertice at a similar position)
                if (Vector3.SqrMagnitude(cleanedVertices[j] - intersectionVertices[i]) < 0.1f)
                {
                    hasFoundNearbyVertice = true;
                    break;
                }
            }

            if (!hasFoundNearbyVertice)
            {
                cleanedVertices.Add(intersectionVertices[i]);
            }
        }
        return cleanedVertices;
    }

    private void DisplayVerticesOrderHeight(List<Vector3> verticesList, Color color)
    {
        float length = 0.1f;
        for (int i = 0; i < verticesList.Count; i++)
        {
            Debug.DrawRay(verticesList[i], Vector3.up * length, color);
            length += 0.2f;
        }
    }


    private void GenerateFoamSkirt(List<Vector3> intersectionVertices)
    {
        List<Vector3> cleanedVertices = CleanVertices(intersectionVertices);

        if (cleanedVertices.Count >= 4)
        {
            List<Vector3> sortedVertices = ConvexHullMath.SortVerticesConvexHull(cleanedVertices);
            List<Vector3> finalVertices = AddVertices(sortedVertices);
            DisplayVerticesOrderHeight(finalVertices, Color.green);

            CreateFoamMesh(finalVertices);
        }
    }
}
