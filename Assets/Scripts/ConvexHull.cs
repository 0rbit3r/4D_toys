using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains methods for algorithms on convex hulls
/// </summary>
public static class ConvexHull
{
    /// <summary>
    /// Returns pairs of indexes corresponging to each edge in a set of convex vertices 
    /// </summary>
    /// <param name="facetVerts">Convex vectors in 3D</param>
    /// <returns>array of tuples corresponding to indexes of given vectors</returns>
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

                    if (plane1.distance == 0 && Vector3.Distance(plane1.normal, new Vector3(0, 0, 0)) == 0) //Not universal, but works for now (TODO check colinearity)
                    {
                        break;
                    }

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
                if (commonVerts.Length >= 2)
                {
                    for (int i = 0; i < commonVerts.Length - 1; i++)
                    {
                        edges.Add((commonVerts[i], commonVerts[i + 1]));
                    }
                }
            }
        }

        return edges.ToArray();
    }

    /// <summary>
    /// Removes duplicates from list of faces
    /// </summary>
    /// <param name="faces">List of Faces  where each face is ordered (ascending) array of indexes</param>
    static void RemoveDuplicates(List<int[]> faces)
    {
        for (int i = 0; i < faces.Count - 1; i++)
        {
            for (int j = i + 1; j < faces.Count; j++)
            {
                if (AreSame(faces[i], faces[j]))
                {
                    faces.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    /// <summary>
    /// Compares two int arrays
    /// </summary>
    /// <param name="arr1"></param>
    /// <param name="arr2"></param>
    /// <returns>True if the contents of two given arrays are identical</returns>
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

    /// <summary>
    /// Returns crossesction of two sets (Used to get common edges between two faces)
    /// </summary>
    /// <param name="face1"></param>
    /// <param name="face2"></param>
    /// <returns></returns>
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
