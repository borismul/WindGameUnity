using UnityEngine;
using System.Collections;

public class ThreadVector3 {

    public float x;
    public float y;
    public float z;

    public ThreadVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public ThreadVector3(Vector3 vector3)
    {
        this.x = vector3.x;
        this.y = vector3.y;
        this.z = vector3.z;
    }

    public static float Dot(ThreadVector3 a, ThreadVector3 b)
    {
        float dot = 0;
        dot += a.x * b.x;
        dot += a.y * b.y;
        dot += a.z * b.z;

        return dot;
    }

    public Vector3 GetVector3()
    {
        Vector3 vector3 = new Vector3(x, y, z);
        return vector3;
    }
}

public class ThreadVector2
{
    public float x;
    public float y;

    public ThreadVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public ThreadVector2(Vector2 vector2)
    {
        this.x = vector2.x;
        this.y = vector2.y;
    }

    public static float Dot(ThreadVector2 a, ThreadVector2 b)
    {
        float dot = 0;
        dot += a.x * b.x;
        dot += a.y * b.y;

        return dot;
    }

    public Vector2 GetVector2()
    {
        Vector2 vector3 = new Vector2(x, y);
        return vector3;
    }
}
