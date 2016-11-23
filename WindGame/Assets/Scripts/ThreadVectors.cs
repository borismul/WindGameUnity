using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public Vector3 ToVector3()
    {
        Vector3 vector3 = new Vector3(x, y, z);
        return vector3;
    }

    public static float Dot(ThreadVector3 a, ThreadVector3 b)
    {
        float dot = 0;
        dot += a.x * b.x;
        dot += a.y * b.y;
        dot += a.z * b.z;

        return dot;
    }

    public static float Magnitude(ThreadVector3 a)
    {
        return Mathf.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
    }

    public static float Distance(ThreadVector3 a, ThreadVector3 b)
    {
        return ThreadVector3.Magnitude(a - b);
    }

    public static ThreadVector3[] ToThreadVectorArray(Vector3[] vector3Array)
    {
        ThreadVector3[] threadVector3Array = new ThreadVector3[vector3Array.Length];

        for(int i = 0; i<vector3Array.Length;i++)
        {
            Vector3 vec3 = vector3Array[i];
            threadVector3Array[i] = new ThreadVector3(vec3.x, vec3.y, vec3.z);
        }

        return threadVector3Array;
    }

    public static ThreadVector3[] ToThreadVectorArray(List<Vector3> vector3List)
    {
        ThreadVector3[] threadVector3Array = new ThreadVector3[vector3List.Count];

        for (int i = 0; i < vector3List.Count; i++)
        {
            Vector3 vec3 = vector3List[i];
            threadVector3Array[i] = new ThreadVector3(vec3.x, vec3.y, vec3.z);
        }

        return threadVector3Array;
    }

    public static List<ThreadVector3> ToThreadVectorList(List<Vector3> vector3List)
    {
        List<ThreadVector3> threadVector3List = new List<ThreadVector3>();

        for (int i = 0; i < vector3List.Count; i++)
        {
            Vector3 vec3 = vector3List[i];
            threadVector3List.Add(new ThreadVector3(vec3.x, vec3.y, vec3.z));
        }

        return threadVector3List;
    }

    public static List<ThreadVector3> ToThreadVectorList(Vector3[] vector3Array)
    {
        List<ThreadVector3> threadVector3List = new List<ThreadVector3>();

        for (int i = 0; i < vector3Array.Length; i++)
        {
            Vector3 vec3 = vector3Array[i];
            threadVector3List.Add(new ThreadVector3(vec3.x, vec3.y, vec3.z));
        }

        return threadVector3List;
    }

    public static Vector3[] ToVectorArray(ThreadVector3[] vector3Array)
    {
        Vector3[] threadVector3Array = new Vector3[vector3Array.Length];

        for (int i = 0; i < vector3Array.Length; i++)
        {
            ThreadVector3 vec3 = vector3Array[i];
            threadVector3Array[i] = new Vector3(vec3.x, vec3.y, vec3.z);
        }

        return threadVector3Array;
    }

    public static Vector3[] ToVectorArray(List<ThreadVector3> vector3List)
    {
        Vector3[] threadVector3Array = new Vector3[vector3List.Count];

        for (int i = 0; i < vector3List.Count; i++)
        {
            ThreadVector3 vec3 = vector3List[i];
            threadVector3Array[i] = new Vector3(vec3.x, vec3.y, vec3.z);
        }

        return threadVector3Array;
    }

    public static List<Vector3> ToVectorList(List<ThreadVector3> vector3List)
    {
        List<Vector3> threadVector3List = new List<Vector3>();

        for (int i = 0; i < vector3List.Count; i++)
        {
            ThreadVector3 vec3 = vector3List[i];
            threadVector3List.Add(new Vector3(vec3.x, vec3.y, vec3.z));
        }

        return threadVector3List;
    }

    public static List<Vector3> ToVectorList(ThreadVector3[] vector3Array)
    {
        List<Vector3> threadVector3List = new List<Vector3>();

        for (int i = 0; i < vector3Array.Length; i++)
        {
            ThreadVector3 vec3 = vector3Array[i];
            threadVector3List.Add(new Vector3(vec3.x, vec3.y, vec3.z));
        }

        return threadVector3List;
    }

    public static ThreadVector3 operator +(ThreadVector3 a, ThreadVector3 b) 
    {
        float x = a.x + b.x;
        float y = a.y + b.y;
        float z = a.z + b.z;
        ThreadVector3 temp = new ThreadVector3(x, y, z);
        return temp;
    }

    public static ThreadVector3 operator -(ThreadVector3 a, ThreadVector3 b)
    {
        float x = a.x - b.x;
        float y = a.y - b.y;
        float z = a.z - b.z;
        ThreadVector3 temp = new ThreadVector3(x, y, z);
        return temp;
    }

    public override string ToString()
    {
        string temp = "ThreadedVector3(" + x + ", " + y + ", " + z +").";
        return temp;
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

    public Vector2 ToVector2()
    {
        Vector2 vector3 = new Vector2(x, y);
        return vector3;
    }

    public static ThreadVector2[] ToThreadVectorArray(Vector2[] vector2Array)
    {
        ThreadVector2[] threadVector2Array = new ThreadVector2[vector2Array.Length];

        for (int i = 0; i < vector2Array.Length; i++)
        {
            Vector2 vec2 = vector2Array[i];
            threadVector2Array[i] = new ThreadVector2(vec2.x, vec2.y);
        }

        return threadVector2Array;
    }

    public static ThreadVector2[] ToThreadVectorArray(List<Vector2> vector2List)
    {
        ThreadVector2[] threadVector2Array = new ThreadVector2[vector2List.Count];

        for (int i = 0; i < vector2List.Count; i++)
        {
            Vector2 vec2 = vector2List[i];
            threadVector2Array[i] = new ThreadVector2(vec2.x, vec2.y);
        }

        return threadVector2Array;
    }

    public static List<ThreadVector2> ToThreadVectorList(List<Vector2> vector2List)
    {
        List<ThreadVector2> threadVector2List = new List<ThreadVector2>();

        for (int i = 0; i < vector2List.Count; i++)
        {
            Vector2 vec2 = vector2List[i];
            threadVector2List.Add(new ThreadVector2(vec2.x, vec2.y));
        }

        return threadVector2List;
    }

    public static List<ThreadVector2> ToThreadVectorList(Vector2[] vector2Array)
    {
        List<ThreadVector2> threadVector2List = new List<ThreadVector2>();

        for (int i = 0; i < vector2Array.Length; i++)
        {
            Vector2 vec2 = vector2Array[i];
            threadVector2List.Add(new ThreadVector2(vec2.x, vec2.y));
        }

        return threadVector2List;
    }

    public static Vector2[] ToVectorArray(ThreadVector2[] vector2Array)
    {
        Vector2[] threadVector2Array = new Vector2[vector2Array.Length];
        ThreadVector2 vec2;
        for (int i = 0; i < vector2Array.Length; i++)
        {
            vec2 = vector2Array[i];
            threadVector2Array[i] = new Vector2(vec2.x, vec2.y);
        }

        return threadVector2Array;
    }

    public static Vector2[] ToVectorArray(List<ThreadVector2> vector2List)
    {
        Vector2[] threadVector2Array = new Vector2[vector2List.Count];

        for (int i = 0; i < vector2List.Count; i++)
        {
            ThreadVector2 vec2 = vector2List[i];
            threadVector2Array[i] = new Vector2(vec2.x, vec2.y);
        }

        return threadVector2Array;
    }

    public static List<Vector2> ToVectorList(List<ThreadVector2> vector2List)
    {
        List<Vector2> threadVector2List = new List<Vector2>();

        for (int i = 0; i < vector2List.Count; i++)
        {
            ThreadVector2 vec2 = vector2List[i];
            threadVector2List.Add(new Vector2(vec2.x, vec2.y));
        }

        return threadVector2List;
    }

    public static List<Vector2> ToVectorList(ThreadVector2[] vector2Array)
    {
        List<Vector2> threadVector2List = new List<Vector2>();

        for (int i = 0; i < vector2Array.Length; i++)
        {
            ThreadVector2 vec2 = vector2Array[i];
            threadVector2List.Add(new Vector2(vec2.x, vec2.y));
        }

        return threadVector2List;
    }
}
