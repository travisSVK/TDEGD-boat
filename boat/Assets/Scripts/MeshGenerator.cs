using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshGenerator : MonoBehaviour
{
    private MeshFilter m_MeshFilter = null;
    private MeshRenderer m_MeshRenderer = null;
    private Mesh m_Mesh = null;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<Vector2> m_UVs = new List<Vector2>();
    private List<int> m_Indices = new List<int>();

    public bool castShadows
    {
        get
        {
            return m_MeshRenderer.shadowCastingMode == ShadowCastingMode.On ||
                m_MeshRenderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly ||
                m_MeshRenderer.shadowCastingMode == ShadowCastingMode.TwoSided;
        }

        set
        {
            m_MeshRenderer.shadowCastingMode = value ? ShadowCastingMode.On : ShadowCastingMode.Off;
        }
    }

    public Material material
    {
        get
        {
            return m_MeshRenderer.material;
        }

        set
        {
            m_MeshRenderer.material = value;
        }
    }

    public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        int current = m_Vertices.Count;

        m_Vertices.Add(p0);
        m_Vertices.Add(p1);
        m_Vertices.Add(p2);

        m_UVs.Add(Vector2.zero);
        m_UVs.Add(Vector2.zero);
        m_UVs.Add(Vector2.zero);

        m_Indices.Add(current + 0);
        m_Indices.Add(current + 1);
        m_Indices.Add(current + 2);
    }

    public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector2 uv0, Vector3 uv1, Vector3 uv2)
    {
        int current = m_Vertices.Count;

        m_Vertices.Add(p0);
        m_Vertices.Add(p1);
        m_Vertices.Add(p2);

        m_UVs.Add(uv0);
        m_UVs.Add(uv1);
        m_UVs.Add(uv2);

        m_Indices.Add(current + 0);
        m_Indices.Add(current + 1);
        m_Indices.Add(current + 2);
    }

    public void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int current = m_Vertices.Count;

        m_Vertices.Add(p0);
        m_Vertices.Add(p1);
        m_Vertices.Add(p3);

        m_Vertices.Add(p1);
        m_Vertices.Add(p2);
        m_Vertices.Add(p3);

        m_UVs.Add(Vector2.zero);
        m_UVs.Add(Vector2.zero);
        m_UVs.Add(Vector2.zero);

        m_UVs.Add(Vector2.zero);
        m_UVs.Add(Vector2.zero);
        m_UVs.Add(Vector2.zero);

        m_Indices.Add(current + 0);
        m_Indices.Add(current + 1);
        m_Indices.Add(current + 2);

        m_Indices.Add(current + 3);
        m_Indices.Add(current + 4);
        m_Indices.Add(current + 5);
    }

    public void Clear()
    {
        m_Vertices.Clear();
        m_UVs.Clear();
        m_Indices.Clear();
    }

    public void Refresh()
    {
        enabled = true;
    }

    private void Awake()
    {
        m_MeshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (!m_MeshFilter)
        {
            m_MeshFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        }

        m_MeshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        if (!m_MeshRenderer)
        {
            m_MeshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        }

        m_Mesh = m_MeshFilter.mesh;
        if (!m_Mesh)
        {
            m_Mesh = new Mesh();
            m_MeshFilter.mesh = m_Mesh;
        }
    }

    private void LateUpdate()
    {
        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.uv = m_UVs.ToArray();
        m_Mesh.triangles = m_Indices.ToArray();
        m_Mesh.RecalculateNormals();
        m_Mesh.RecalculateTangents();
        m_Mesh.RecalculateBounds();

        enabled = false;
    }
}
