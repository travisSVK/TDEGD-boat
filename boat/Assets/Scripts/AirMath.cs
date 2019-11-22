using UnityEngine;

public static class AirMath
{
    /*
    * Similar to WaterMath.CalculateViscousWaterResistance(), but not tangential force, just opposite to velocity.
    * https://www.habrador.com/tutorials/unity-boat-tutorial/5-resistance-forces/
    */
    public static Vector3 CalculateAirResistance(float rho, Triangle triangle, float airDrag)
    {
        // R_air = 1/2 * rho * v^2 * A_p * C_air
        // rho - air density
        // v - speed of ship
        // A_p - area affected
        // C_r - coefficient of air resistance (drag coefficient)

        float normalVelocityCos = Vector3.Dot(triangle.normal, triangle.velocity.normalized);
        // the air pressure only applies to front-facing triangles
        if (normalVelocityCos < 0.0f)
        {
            return Vector3.zero;
        }
        
        // opposite direction than velocity
        return -0.5f * rho * triangle.velocity.magnitude * triangle.velocity * triangle.area * airDrag;
    }
}
