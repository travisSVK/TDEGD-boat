using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterCheck : MonoBehaviour
{
    [SerializeField]
    private Transform[] m_ForcePoints = null;
    
    [SerializeField]
    private Vector3 m_Buoyancy = new Vector3(0.0f, Physics.gravity.y, 0.0f);
    
    private Water m_Water = null;
    private Rigidbody m_Rigidbody = null;

    private void Start()
    {
        m_Water = FindObjectOfType<Water>();
        m_Rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
    }

    private void FixedUpdate()
    {
        foreach (Transform forcePoint in m_ForcePoints)
        {
            Vector3 forcePointPosition = forcePoint.position;
            float depth = m_Water.GetHeight(forcePointPosition.x, forcePointPosition.z) - forcePointPosition.y;
            Debug.Log(m_Buoyancy * depth);
            m_Rigidbody.AddForceAtPosition(m_Buoyancy * depth, forcePointPosition, ForceMode.Force);
            //if (depth > 0.0f)
            //{
            //    Debug.Log(m_Buoyancy + m_Buoyancy * depth);
            //    Debug.Log(depth);
            //    m_Rigidbody.AddForceAtPosition(1027f * m_Buoyancy * depth, forcePointPosition, ForceMode.Force);
            //}
            //else
            //{
            //    m_Rigidbody.AddForceAtPosition(m_Buoyancy, forcePointPosition, ForceMode.Force);
            //}
        }
    }
}
