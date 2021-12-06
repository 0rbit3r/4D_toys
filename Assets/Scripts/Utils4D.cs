using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Utils4D
{
    public static Vector3[] ProjectTo3D(Vector4[] vertices)
    {
        Vector3[] toReturn = new Vector3[vertices.Length];
        //Trying to remove one dimension so that all new points are unique
        for (int excesDim = 0; excesDim < 4; excesDim++)
        {
            for (int v = 0; v < vertices.Length; v++)
            {
                List<float> newCoors = new List<float>();
                for (int j = 0; j < 4; j++)
                {
                    if (j != excesDim)
                    {
                        newCoors.Add(vertices[v][j]);
                    }
                }
                toReturn[v] = new Vector3(newCoors[0], newCoors[1], newCoors[2]);
            }

            //If all points are unique, return new points
            if (!toReturn.GroupBy(x => x).Where(g => g.Count() > 1).Any())
            {
                return toReturn;
            }
        }

        return null;
    }
}
