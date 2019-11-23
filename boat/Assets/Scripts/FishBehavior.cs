using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    [SerializeField] private float m_MaxForce = 5.0f;
    [SerializeField] private float m_NormalForce = 40.0f;

    private Water m_Water = null;
    private Rigidbody m_Rigidbody = null;

    private void Awake()
    {
        m_Rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        m_Water = FindObjectOfType(typeof(Water)) as Water;
    }

    private void FixedUpdate()
    {
        Vector3 point = m_Rigidbody.position + transform.right * m_NormalForce * Time.fixedDeltaTime;
        float heightCheck = m_Water.GetFloorDepth(point.x, point.z) + 2.0f;
        if (heightCheck > m_Rigidbody.position.y)
        {
            m_Rigidbody.AddForce(Vector3.up * 1000);
            m_Rigidbody.AddForce(transform.right * m_NormalForce);
        }
    }
}
