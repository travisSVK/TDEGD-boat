using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    private float m_WaterDensity = 1027.0f;
    [SerializeField] private float m_BouyancyCoeficient = 0.004f;
    [SerializeField] private float distanceToSurfaceThreshold = 0.1f;
    [SerializeField] private float normalThreshold = 0.0f;
    
    private BuoyancyMesh m_BoatMesh;
    private Rigidbody m_RigidBody;
    private bool m_StartPositionSet = false;

    private void Start()
    {
        m_RigidBody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        m_BoatMesh = GetComponent(typeof(BuoyancyMesh)) as BuoyancyMesh;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (m_BoatMesh.underWaterTriangles.Count > 0)
        {
            // Get all triangles
            List<Triangle> underWaterTriangles = m_BoatMesh.underWaterTriangles;

            foreach (Triangle triangle in underWaterTriangles)
            {
                Vector3 buoyancyForce = CalculateBuoyancy(m_WaterDensity, triangle);

                // Add the force to the boat
                m_RigidBody.AddForceAtPosition(buoyancyForce, triangle.center);

                // debug Normal
                //Debug.DrawRay(triangle.center, triangle.normal * 3f, Color.white);

                // debug buoyancy
                Debug.DrawRay(triangle.center, buoyancyForce * 0.001f, Color.blue);
            }
        }
    }

    private Vector3 CalculateBuoyancy(float waterDensity, Triangle triangle)
    {
        // F_buoyancy = rho * g * V
        // rho - density of the liquid(kg/m3)
        // g - gravitational acceleration(9.80 m/s2)
        // V - volume of liquid displaced (m3 or liters, where 1 m3 = 1000 L)

        // V = h * S * n 
        // h - distance to surface
        // S - surface area
        // n - normal to the surface
        Vector3 buoyancyForce = 
            waterDensity * 
            Physics.gravity.y * 
            (triangle.distanceToSurface <= distanceToSurfaceThreshold ? 0.0f : triangle.distanceToSurface) * 
            triangle.area * 
            //triangle.normal * m_BouyancyCoeficient;
            (triangle.normal.y <= 0.0f ? triangle.normal : Vector3.zero) * m_BouyancyCoeficient;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}
