using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float m_Acceleration = 1.0f;

    private Vector3 m_accelerationDirection = Vector3.zero;
    private Rigidbody m_RigidBody;
    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = Input.GetAxis("Horizontal");
        m_accelerationDirection = new Vector3(velocity, 0.0f, 0.0f);
    }

    private void FixedUpdate()
    {
        m_RigidBody.AddForce(m_accelerationDirection * m_Acceleration, ForceMode.Acceleration);
    }
}
