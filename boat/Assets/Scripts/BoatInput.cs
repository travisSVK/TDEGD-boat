using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatInput : MonoBehaviour
{
    [SerializeField] private string m_AccelerationAxisName = "";

    private BoatEngine m_BoatEngine = null;

    private void Awake()
    {
        m_BoatEngine = GetComponent(typeof(BoatEngine)) as BoatEngine;
    }

    private void Update()
    {
        float input = Input.GetAxis(m_AccelerationAxisName);
        m_BoatEngine.Accelerate(input);
    }
}
