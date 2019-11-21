using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math
{
    public static float ResistanceCoefficient(float rho, float velocity, float length)
    {
        // Cf(Rn) = 0.0075 / (log10(Rn) - 2)^2
        // Rn = (V * L) / nu
        // V - speed of the body
        // L - length of the sumbmerged body
        // nu - viscosity of the fluid [m^2 / s]

        // Viscocity at 20 degrees celcius:
        float nu = 0.000001f;

        //Reynolds number
        float Rn = (velocity * length) / nu;

        //The resistance coefficient
        float Cf = 0.075f / Mathf.Pow((Mathf.Log10(Rn) - 2f), 2f);

        return Cf;
    }

    public static Vector3 CalculateBuoyancy(Triangle triangle, float waterDensity, float distanceToSurfaceThreshold, float buoyancyCoeficient)
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
            (triangle.normal.y <= 0.0f ? triangle.normal : Vector3.zero) * buoyancyCoeficient;
        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }

    public static Vector3 CalculateViscousWaterResistance(float waterDensity, Triangle triangle, float Cf)
    {
        // F = 1/2 * rho * Cf * v * S
        // rho - density of the water
        // v - tangent speed of the flow at the triangle
        // S - triangle area
        // Cf - Coefficient of frictional resistance

        // direction of tangent of the velocity is projection on the plane:
        // vt =  nromal × (velocity × normal / |normal|) / normal.magnitude
        // http://www.euclideanspace.com/maths/geometry/elements/plane/lineOnPlane/
        // also, we need to change the direction, since water flows the other way around
        Vector3 velocityTangent = (
                Vector3.Cross(triangle.normal, (Vector3.Cross(triangle.velocity, triangle.normal) / triangle.normal.magnitude))
                / triangle.normal.magnitude
            ).normalized * -1.0f;
        
        // now we take the speed at the triangle center and use its oposite direction for the flow
        Vector3 waterFlowVelocity = triangle.velocity.magnitude * velocityTangent;

        //The final resistance force
        return 0.5f * waterDensity * waterFlowVelocity.magnitude * waterFlowVelocity * triangle.area * Cf;
    }
}
