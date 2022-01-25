using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ConvexHull
{
    public static (int, int)[] GetConvexEdges(Vector3[] facetVerts)
    {
        const float curvatureTolerance = 0.01f;

        List<int[]> validFaces = new List<int[]>();

        var edges = new List<(int, int)>();

        for (int i = 0; i < facetVerts.Length - 2; i++)
        {
            for (int j = i + 1; j < facetVerts.Length - 1; j++)
            {
                for (int k = j + 1; k < facetVerts.Length; k++)
                {
                    Plane plane1 = new Plane(facetVerts[i], facetVerts[j], facetVerts[k]);
                    Plane plane2 = new Plane(facetVerts[i], facetVerts[k], facetVerts[j]);

                    var isValid1 = true;
                    var isValid2 = true;

                    var faceVerts = new List<int>();

                    for (int l = 0; l < facetVerts.Length; l++)
                    {
                        if (facetVerts[l] != facetVerts[i] && facetVerts[l] != facetVerts[j] && facetVerts[l] != facetVerts[k])
                        {
                            var dist1 = plane1.GetDistanceToPoint(facetVerts[l]);
                            var dist2 = plane2.GetDistanceToPoint(facetVerts[l]);

                            if (dist1 > curvatureTolerance)
                            {
                                isValid1 = false;
                            }
                            if (dist2 > curvatureTolerance)
                            {
                                isValid2 = false;
                            }
                            if (Mathf.Abs(dist1) <= curvatureTolerance)
                            {
                                faceVerts.Add(l);
                            }
                            if (!isValid1 && !isValid2)
                            {
                                break;
                            }
                        }
                    }

                    if (isValid1 || isValid2)
                    {
                        faceVerts.Add(i);
                        faceVerts.Add(j);
                        faceVerts.Add(k);

                        faceVerts.Sort();

                        validFaces.Add(faceVerts.ToArray());
                    }
                }
            }
        }

        RemoveDuplicates(validFaces);

        while (validFaces.Count != 0)
        {
            var currentFace = validFaces[0];
            validFaces.RemoveAt(0);

            foreach (var face2 in validFaces)
            {
                int[] commonVerts = GetCommonVertices(face2, currentFace);
                if (commonVerts.Length == 2)
                {
                    edges.Add((commonVerts[0], commonVerts[1]));
                }
            }
        }

        return edges.ToArray();
    }

    static void RemoveDuplicates(List<int[]> faces)
    {
        for (int i = 0; i < faces.Count - 1; i++)
        {
            for (int j = i+1; j < faces.Count; j++)
            {
                if(AreSame(faces[i], faces[j]))
                {
                    faces.RemoveAt(j);
                }
            }
        }
    }
    static bool AreSame(int[] arr1, int[] arr2)
    {
        if (arr1.Length == arr2.Length)
        {
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    static int[] GetCommonVertices(int[] face1, int[] face2)
    {
        List<int> commonVerts = new List<int>();
        foreach (var v1 in face1)
        {
            foreach (var v2 in face2)
            {
                if (v1 == v2)
                {
                    commonVerts.Add(v1);
                }
            }
        }
        return commonVerts.ToArray();
    }
}
