using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoundingBox
{
    public float X1 { get; }
    public float X2 { get; }
    public float Y1 { get; }
    public float Y2 { get; }
    public float Z1 { get; }
    public float Z2 { get; }
    public float W1 { get; }
    public float W2 { get; }

    public bool Intersects(IBoundingBox b2);

    public void Update<T>(Vector4 position, T[] arr);
}

public class ObjectBoundingBox : IBoundingBox
{
    public float X1 { get; private set; }
    public float X2 { get; private set; }
    public float Y1 { get; private set; }
    public float Y2 { get; private set; }
    public float Z1 { get; private set; }
    public float Z2 { get; private set; }
    public float W1 { get; private set; }
    public float W2 { get; private set; }

    public ObjectBoundingBox(Vector4 position, Vector4[] vertices)
    {
        UpdateFromVertices(position, vertices);
    }

    public void Update<T>(Vector4 position, T[] vertices)
    {
        UpdateFromVertices(position, vertices as Vector4[]);
    }

    public void UpdateFromVertices(Vector4 position, Vector4[] vertices)
    {
        X1 = vertices[0].x + position.x;
        X2 = vertices[0].x + position.x;
        Y1 = vertices[0].y + position.y;
        Y2 = vertices[0].y + position.y;
        Z1 = vertices[0].z + position.z;
        Z2 = vertices[0].z + position.z;
        W1 = vertices[0].w + position.w;
        W2 = vertices[0].w + position.w;

        foreach (var v in vertices)
        {
            if (v.x + position.x < X1)
                X1 = v.x + position.x;
            if (v.x + position.x > X2)
                X2 = v.x + position.x;
            if (v.y + position.y < Y1)
                Y1 = v.y + position.y;
            if (v.y + position.y > Y2)
                Y2 = v.y + position.y;
            if (v.z + position.z < Z1)
                Z1 = v.z + position.z;
            if (v.z + position.z > Z2)
                Z2 = v.z + position.z;
            if (v.w + position.w < W1)
                W1 = v.w + position.w;
            if (v.w + position.w > W2)
                W2 = v.w + position.w;
        }
    }

    public bool Intersects(IBoundingBox b2)
    {
        if (X1 < b2.X2 && X2 > b2.X1
            && Y1 < b2.Y2 && Y2 > b2.Y1
            && Z1 < b2.Z2 && Z2 > b2.Z1
            && W1 < b2.W2 && W2 > b2.W1)
        {
            return true;
        }

        return false;
    }
}

public class FacetBoundingBox : IBoundingBox
{
    Vector4[] Vertices;
    public float X1 { get; private set; }
    public float X2 { get; private set; }
    public float Y1 { get; private set; }
    public float Y2 { get; private set; }
    public float Z1 { get; private set; }
    public float Z2 { get; private set; }
    public float W1 { get; private set; }
    public float W2 { get; private set; }

    public FacetBoundingBox(Vector4[] vertices, Vector4 position, int[] indexes)
    {
        Vertices = vertices;

        UpdateFromIndexes(position, indexes);
    }

    public void Update<T>(Vector4 position, T[] indexes)
    {
        UpdateFromIndexes(position, indexes as int[]);
    }

    public void UpdateFromIndexes(Vector4 position, int[] indexes)
    {
        X1 = Vertices[0].x + position.x;
        X2 = Vertices[0].x + position.x;
        Y1 = Vertices[0].y + position.y;
        Y2 = Vertices[0].y + position.y;
        Z1 = Vertices[0].z + position.z;
        Z2 = Vertices[0].z + position.z;
        W1 = Vertices[0].w + position.w;
        W2 = Vertices[0].w + position.w;

        foreach (var v in indexes)
        {
            if (Vertices[v].x + position.x < X1)
                X1 = Vertices[v].x + position.x;
            if (Vertices[v].x + position.x > X2)
                X2 = Vertices[v].x + position.x;
            if (Vertices[v].y + position.y < Y1)
                Y1 = Vertices[v].y + position.y;
            if (Vertices[v].y + position.y > Y2)
                Y2 = Vertices[v].y + position.y;
            if (Vertices[v].z + position.z < Z1)
                Z1 = Vertices[v].z + position.z;
            if (Vertices[v].z + position.z > Z2)
                Z2 = Vertices[v].z + position.z;
            if (Vertices[v].w + position.w < W1)
                W1 = Vertices[v].w + position.w;
            if (Vertices[v].w + position.w > W2)
                W2 = Vertices[v].w + position.w;
        }
    }

    public bool Intersects(IBoundingBox b2)
    {
        if (X1 < b2.X2 && X2 > b2.X1
            && Y1 < b2.Y2 && Y2 > b2.Y1
            && Z1 < b2.Z2 && Z2 > b2.Z1
            && W1 < b2.W2 && W2 > b2.W1)
        {
            return true;
        }

        return false;
    }

}