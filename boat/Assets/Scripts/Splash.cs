using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField]
    private Mesh m_Mesh = null;

    [SerializeField]
    private GameObject m_WaterSplashEffect = null;

    [SerializeField]
    private float m_Threshold = 0.02f;

    private Water m_Water = null;
    private List<Vector3> m_Vertices = new List<Vector3>();
    private List<bool> m_IsUnderwater = new List<bool>();

    private void Awake()
    {
        if (!m_Mesh)
        {
            Debug.LogError("No mesh attached for Splash.");
            enabled = false;
            return;
        }

        for (int i = 0; i < m_Mesh.vertexCount; ++i)
        {
            Vector3 vertex = transform.TransformPoint(m_Mesh.vertices[i]);
            m_Vertices.Add(vertex);
            m_IsUnderwater.Add(false);
        }
    }

    private void FixedUpdate()
    {
        if (!m_Water)
        {
            m_Water = FindObjectOfType(typeof(Water)) as Water;
            if (!m_Water)
            {
                return;
            }
        }

        for (int i = 0; i < m_Mesh.vertexCount; ++i)
        {
            Vector3 vertex = transform.TransformPoint(m_Mesh.vertices[i]);
            bool isUnderwater = m_Water.IsUnderwater(vertex);
            if (isUnderwater != m_IsUnderwater[i])
            {
                Vector3 velocity = (transform.TransformPoint(m_Mesh.vertices[i]) - m_Vertices[i]) / Time.fixedDeltaTime;
                float magnitude = velocity.magnitude;
                if (magnitude >= m_Threshold)
                {
                    Destroy(Instantiate(m_WaterSplashEffect, vertex, Quaternion.identity), 0.3f);
                }
            }

            m_Vertices[i] = vertex;
            m_IsUnderwater[i] = isUnderwater;
        }
    }
}
