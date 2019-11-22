using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaterMath
{
    /*
    * Water resistance coeficient
    * https://gamasutra.com/view/news/263237/Water_interaction_model_for_boats_in_video_games_Part_2.php
    */
    public static float ResistanceCoefficient(float rho, float velocity, float length)
    {
        // Cf(Rn) = 0.0075 / (log10(Rn) - 2)^2
        // Rn = (V * L) / nu
        // V - speed of the body
        // L - length of the sumbmerged body
        // nu - viscosity of the fluid [m^2 / s]

        // Viscocity at 20 degrees celcius:
        float nu = 0.000001f;

        // Reynolds number.
        float Rn = (velocity * length) / nu;

        // The resistance coefficient.
        float Cf = 0.075f / Mathf.Pow((Mathf.Log10(Rn) - 2f), 2f);

        return Cf;
    }

    /*
    * Buoyancy force
    * https://gamasutra.com/view/news/263237/Water_interaction_model_for_boats_in_video_games_Part_2.php
    */
    public static Vector3 CalculateBuoyancy(Triangle triangle, float waterDensity, float distanceToSurfaceThreshold, float normalThreshold, float buoyancyCoeficient)
    {
        // F_buoyancy = rho * g * V.
        // rho - density of the liquid(kg/m3).
        // g - gravitational acceleration(9.80 m/s2).
        // V - volume of liquid displaced (m3 or liters, where 1 m3 = 1000 L).

        // V = h * S * n .
        // h - distance to surface.
        // S - surface area.
        // n - normal to the surface.
        Vector3 buoyancyForce =
            waterDensity *
            Physics.gravity.y *
            (triangle.distanceToSurface <= distanceToSurfaceThreshold ? 0.0f : triangle.distanceToSurface) *
            triangle.area *
            //triangle.normal * m_BouyancyCoeficient;
            (triangle.normal.y <= normalThreshold ? triangle.normal : Vector3.zero) * buoyancyCoeficient;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }

    /*
    * Viscous water resistance, ie. water draged along the surface of the boat (tangential forces)
    * https://gamasutra.com/view/news/263237/Water_interaction_model_for_boats_in_video_games_Part_2.php
    */
    public static Vector3 CalculateViscousWaterResistance(float waterDensity, Triangle triangle, float Cf)
    {
        // F = 1/2 * rho * Cf * v * S.
        // rho - density of the water.
        // v - tangent speed of the flow at the triangle.
        // S - triangle area.
        // Cf - Coefficient of frictional resistance.

        // direction of tangent of the velocity is projection on the plane:
        // vt =  nromal × (velocity × normal / |normal|) / normal.magnitude.
        // http://www.euclideanspace.com/maths/geometry/elements/plane/lineOnPlane/
        // also, we need to change the direction, since water flows the other way around.
        Vector3 velocityTangent = (
                Vector3.Cross(triangle.normal, (Vector3.Cross(triangle.velocity, triangle.normal) / triangle.normal.magnitude))
                / triangle.normal.magnitude
            ).normalized * -1.0f;
        
        // Now we take the speed at the triangle center and use its oposite direction for the flow.
        Vector3 waterFlowVelocity = triangle.velocity.magnitude * velocityTangent;

        // The final resistance force.
        return 0.5f * waterDensity * waterFlowVelocity.magnitude * waterFlowVelocity * triangle.area * Cf;
    }

    /*
    * This is unrealistic force, made up to simulate the wave breaking, spray making resistances, etc.
    * It is applied depending on the triangle normal (as opposed to viscous drag) of the underwater triangles
    * and boat velocity. Modifies the boat turning behaviour depending on speed.
    * https://gamasutra.com/view/news/263237/Water_interaction_model_for_boats_in_video_games_Part_2.php
    */
    public static Vector3 CalculatePressureDrag(Triangle triangle, float Cpd1, float Cpd2, float Csd1, float Csd2, float fp, float fs)
    {
        // Formula:
        // F = -(Cpd1 * (vi/vr) + Cpd2 * (vi/vr)^2) * Si * (dot(normal.normalized, velocity.normalized))^fp * normal,
        // if cosine between veocity and normal is positive (pressure)
        // where Cpd1: linear pressure drag coeficient; vi/vr: speed at triangle center / reference speed == 1;
        // Cpd2: quadratic pressure drag coeficient; Si: triangle area; fp: pressure falloff power (how fast the drag falls off
        // depending on cosine between velocity and normal)
        //
        // F = (Csd1 * (vi/vr) + Csd2 * (vi/vr)^2) * Si * (dot(normal.normalized, velocity.normalized))^fs * normal,
        // if cosine between veocity and normal is negative (suction)
        // where Csd1 and Csd2: linear and quadratic suction drag coeficients; fs: suction falloff power

        float normalVelocityCos = Vector3.Dot(triangle.normal, triangle.velocity.normalized); 
        if (normalVelocityCos > 0.0f)
        { 
            return -(Cpd1 + Cpd2) * triangle.area * Mathf.Pow(normalVelocityCos, fp) * triangle.normal;
        }
        // normalVelocityCos needs to be absolute value here cause the force will apply in the other direction
        return (Csd1 + Csd2) * triangle.area * Mathf.Pow(Mathf.Abs(normalVelocityCos), fs) * triangle.normal;
    }

}
