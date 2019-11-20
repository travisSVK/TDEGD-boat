using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField]
    private float m_WaterDensity = 1027.0f;

    //Mesh for debugging
    private Mesh m_Mesh;
    private BoatMesh m_BoatMesh;
    private Rigidbody m_RigidBody;
    private Water m_Water;
    private bool m_StartPositionSet = false;

    private void Start()
    {
        m_RigidBody = gameObject.GetComponent<Rigidbody>();
        m_BoatMesh = new BoatMesh(gameObject);
        m_Water = FindObjectOfType(typeof(Water)) as Water;
        
    }

    private void Update()
    {
        if (!m_StartPositionSet)
        {
            transform.position = new Vector3(transform.position.x, m_Water.GetHeight(transform.position.x, transform.position.z), transform.position.z);
            m_StartPositionSet = true;
        }
        m_BoatMesh.GenerateUnderwaterMesh(m_Water);
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
                Debug.DrawRay(triangle.center, buoyancyForce.normalized * -3f, Color.blue);
            }
        }
    }

    private Vector3 CalculateBuoyancy(float waterDensity, Triangle triangle)
    {
        //Buoyancy is a hydrostatic force - it's there even if the water isn't flowing or if the boat stays still

        // F_buoyancy = rho * g * V
        // rho - density of the liquid(kg/m3)
        // g - gravitational acceleration(9.80 m/s2)
        // V - volume of liquid displaced (m3 or liters, where 1 m3 = 1000 L)

        // V = h * S * n 
        // h - distance to surface
        // S - surface area
        // n - normal to the surface
        Vector3 buoyancyForce = waterDensity * Physics.gravity.y * triangle.distanceToSurface * triangle.area * triangle.normal * 0.00046f;
        //(triangle.normal.y <= 0.0f ? triangle.normal : Vector3.zero) * 0.01f;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}
