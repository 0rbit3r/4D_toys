using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a "face" of a 4D object - In reality a 3D convex set of vertices suspended in 4D space.
/// </summary>
public class Facet : MonoBehaviour
{
    Object4D Parent4DObject;

    //Indexes pointing at Vertices of parent object
    public int[] FacetVertIndexes;

    private (int, int)[] FacetEdges;

    Mesh Mesh;

    Material Material;

    public void Init(int[] vertices, Color color, Object4D parent)
    {
        FacetVertIndexes = vertices;
        Parent4DObject = parent;

        InitalizeEdges();


        Material = new Material(Resources.Load<Material>("Materials/DefaultFacet"));
        Material.color = color;

        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().material = Material;


        gameObject.AddComponent<MeshFilter>();

        Mesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = Mesh;

    }

    /// <summary>
    /// Updates the sliced part of the object that is to be rendered in given hyperplane
    /// </summary>
    /// <param name="plane"></param>
    public void RenderIn(Hyperplane plane)
    {
        Mesh.Clear();
        var vectors = CutWith(Parent4DObject.RotatedVertices, plane);
        UpdateMesh(vectors);

    }

    /// <summary>
    /// Updates Mesh Renderer
    /// </summary>
    /// <param name="vertsToRender">A convex set of points laying in a plane.</param>
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

    }

    /// <summary>
    /// Initializes the FacetEdges of the object
    /// </summary>
    private void InitalizeEdges()
    {
        Vector4[] vertices = GetDereferencedVertexList();
        Vector3[] flattenedVertices = Utils4D.ProjectTo3D(vertices);
        FacetEdges = ConvexHull.GetConvexEdges(flattenedVertices);
    }

    /// <summary>
    /// Returns cross-section with hyperplane
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="hyperplane"></param>
    /// <returns></returns>
    public Vector3[] CutWith(Vector4[] vertices, Hyperplane hyperplane)
    {
        List<Vector3> resultingPolygon = new List<Vector3>();

        foreach (var edge in FacetEdges)
        {
            var bod1 = vertices[FacetVertIndexes[edge.Item1]];
            var bod2 = vertices[FacetVertIndexes[edge.Item2]];
            var distance1 = hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item1]]);
            var distance2 = hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item2]]);


            if ((hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item1]]) < 0 && hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item2]]) >= 0)
                || (hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item1]]) >= 0 && hyperplane.DistanceTo(vertices[FacetVertIndexes[edge.Item2]]) < 0))
            {
                resultingPolygon.Add(hyperplane.CrossSectionWithLine(vertices[FacetVertIndexes[edge.Item1]], vertices[FacetVertIndexes[edge.Item2]]));
            }
        }

        resultingPolygon = resultingPolygon.Distinct().ToList();
        if (resultingPolygon.Count >= 3)
        {
            var baseVector = resultingPolygon[1] - resultingPolygon[0];
            resultingPolygon.Sort((x, y) => Vector3.Angle(baseVector, x - resultingPolygon[0]).CompareTo(Vector3.Angle(baseVector, y - resultingPolygon[0]))); //Possibly needs more thought
        }

        return resultingPolygon.ToArray();
    }

    /// <summary>
    /// Returns actual 4D vertices making up the Facet.
    /// </summary>
    /// <returns></returns>
    public Vector4[] GetDereferencedVertexList()
    {
        var toReturn = new Vector4[FacetVertIndexes.Length];

        for (int i = 0; i < toReturn.Length; i++)
        {
            toReturn[i] = Parent4DObject.Vertices[FacetVertIndexes[i]];
        }

        return toReturn;
    }
}
