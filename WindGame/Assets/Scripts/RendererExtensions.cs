using UnityEngine;

public static class RendererExtensions
{
    static Plane[] planes = new Plane[6];

    static void GetPlanes(Camera camera)
    {
        planes = GeometryUtility.CalculateFrustumPlanes(camera);

    }
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {

        GeometryUtility.CalculateFrustumPlanes(camera, planes);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

}