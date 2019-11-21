using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatEngine : MonoBehaviour
{
    [SerializeField] private Transform m_ForcePoint = null;
    [SerializeField] private float m_PowerFactor = 1.0f;
    [SerializeField] private float m_MaxPower = 10.0f;

    private float m_CurrentPower = 0.0f;
    private Rigidbody m_Rigidbody = null;
    private Water m_Water = null;

    public void Accelerate(float value)
    {
        if (Mathf.Approximately(value, 0.0f))
        {
            m_CurrentPower = 0.0f;
        }
        else
        {
            float currentSpeed = m_Rigidbody.velocity.magnitude;
            if (currentSpeed < 50f && m_CurrentPower < m_MaxPower)
            {
                m_CurrentPower += value * m_PowerFactor * Time.deltaTime;
            }
        }
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        m_Water = FindObjectOfType(typeof(Water)) as Water;
    }

    private void FixedUpdate()
    {
        if (m_Water.IsUnderwater(m_ForcePoint.position))
        {
            Debug.Log(m_CurrentPower);
            Vector3 force = m_ForcePoint.right * m_CurrentPower;
            m_Rigidbody.AddForceAtPosition(force, m_ForcePoint.position);
        }
    }
}
