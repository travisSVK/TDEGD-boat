using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target = null;

    private void LateUpdate()
    {
        transform.position = new Vector3(m_Target.position.x, transform.position.y, transform.position.z);
    }
}
