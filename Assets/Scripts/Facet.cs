using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Facet : MonoBehaviour
{
    Object4D parentObj;

    //Indexes pointing at Vertices of parent object
    public int[] FacetVertIndexes;

    private (int, int)[] FacetEdges;

    Mesh Mesh;

    Material Material;

    public void Init(int[] vertices, Color color, Object4D parent)
    {
        FacetVertIndexes = vertices;
        parentObj = parent;

        FacetEdges = CreateEdges();


        Material = new Material(Resources.Load<Material>("DefaultFacet"));
        Material.color = color;

        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().material = Material;


        gameObject.AddComponent<MeshFilter>();

        Mesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = Mesh;

    }
    public void RenderIn(Hyperplane plane)
    {
        Mesh.Clear();
        var vectors = CutWith(parentObj.RotatedVertices, plane);
        UpdateMesh(vectors);

    }

    private void UpdateMesh(Vector3[] vertsToRender)
    {
        if (vertsToRender.Length > 2)
        {
            Mesh.vertices = vertsToRender;

            var newTriangles = new List<int>();

            for (int i = 1; i < vertsToRender.Length - 1; i++)
            {
                newTriangles.Add(0);
                newTriangles.Add(i);
                newTriangles.Add(i + 1);

                newTriangles.Add(0);
                newTriangles.Add(i + 1);
                newTriangles.Add(i);
            }

            Mesh.triangles = newTriangles.ToArray();
        }


        //List<Vector3> tempMeshVertices = new List<Vector3>(Mesh.vertices);
        //int offset = tempMeshVertices.Count;
        //tempMeshVertices.AddRange(vertsToRender);
        //Mesh.vertices = tempMeshVertices.ToArray();

        //var newTriangles = new List<int>();

        //if (vertsToRender.Length > 2)
        //{
        //    for (int i = 1; i < vertsToRender.Length - 1; i++)
        //    {
        //        newTriangles.Add(offset + 0);
        //        newTriangles.Add(offset + i);
        //        newTriangles.Add(offset + i + 1);

        //        newTriangles.Add(offset + 0);
        //        newTriangles.Add(offset + i + 1);
        //        newTriangles.Add(offset + i);

        //    }
        //}

        //var tempTriangles = new List<int>(Mesh.triangles);
        //tempTriangles.AddRange(newTriangles);
        //Mesh.triangles = tempTriangles.ToArray();

    }

    private (int, int)[] CreateEdges()
    {
        Vector4[] vertices = GetDereferencedVertexList();
        Vector3[] flattenedVertices = Utils4D.ProjectTo3D(vertices);
        return ConvexHull.GetConvexEdges(flattenedVertices);
    }

    public Vector3[] CutWith(Vector4[] vertices, Hyperplane hyperplane)
    {
        List<Vector3> result = new List<Vector3>();

        foreach (var edge in FacetEdges)
        {
            var bod1 = vertices[FacetVertIndexes[edge.Item1]];
            var bod2 = vertices[FacetVertIndexes[edge.Item2]];
            var distance1 = hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item1]]);
            var distance2 = hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item2]]);


            if ((hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item1]]) < 0 && hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item2]]) >= 0)
                || (hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item1]]) >= 0 && hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item2]]) < 0))
            {
                result.Add(hyperplane.CrossSectionWithLine(vertices[FacetVertIndexes[edge.Item1]], vertices[FacetVertIndexes[edge.Item2]]));
            }
        }

        if (result.Count >= 3)
        {
            result = result.Distinct().ToList();
            var baseVector = result[1] - result[0];
            result.Sort((x, y) => Vector3.Angle(baseVector, x - result[0]).CompareTo(Vector3.Angle(baseVector, y - result[0]))); //Possibly needs more thought
        }

        return result.ToArray();
    }

    public Vector4[] GetDereferencedVertexList()
    {
        var toReturn = new Vector4[FacetVertIndexes.Length];

        for (int i = 0; i < toReturn.Length; i++)
        {
            toReturn[i] = parentObj.Vertices[FacetVertIndexes[i]];
        }

        return toReturn;
    }
}
