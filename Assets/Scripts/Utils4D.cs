using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils4D
{
    public static Color[] FacetColors = { Color.white, Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow, Color.gray };
    public static Vector3[] ProjectTo3D(Vector4[] vertices)
    {
        bool[] dimsAreVarying = new bool[4];

        for (int dim = 0; dim < 4; dim++)
        {
            for (int vertex = 1; vertex < vertices.Length; vertex++)
            {

                if (vertices[vertex][dim] != vertices[vertex - 1][dim])
                {
                    dimsAreVarying[dim] = true;
                }
            }
        }

        if (dimsAreVarying.Count(x => true) < 3)
        {
            throw new System.Exception("Facet is planar!");
        }

        int excessDim = -1;
        for (int i = 0; i < 3; i++)
        {
            if (! dimsAreVarying[i])
            {
                excessDim = i;
            }
        }
        excessDim = excessDim == -1 ? 3 : excessDim;

        Vector3[] toReturn = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            float[] flattenedVector = new float[3];
            int flatIndex = 0;
            for (int dim = 0; dim < 4; dim++)
            {
                if(dim != excessDim)
                {
                    flattenedVector[flatIndex++] = vertices[i][dim];
                }
            }
            toReturn[i] = new Vector3(flattenedVector[0], flattenedVector[1], flattenedVector[2]);
        }

        return toReturn;
    }
}
