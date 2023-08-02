using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtension
{
    public static bool Visible(this Camera camera, Collider collider)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }

}