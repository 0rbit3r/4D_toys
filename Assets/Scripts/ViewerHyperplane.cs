using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

/// <summary>
/// Holds position and handles movement of 3D hyperplane, in which players POV sits
/// 
/// Spawns slider used as a VR input
/// </summary>
public class ViewerHyperplane : MonoBehaviour, IHyperplane
{
    public float Offset;

    Slider Slider;

    // Start is called before the first frame update
    void Start()
    {
        Slider = new Slider(this);
        Offset = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Slider.Update();
        Offset = Slider.Value;
    }

    /// <summary>
    /// Gives signed distance from point to the hyperplane.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public float DistanceTo(Vector4 vertex)
    {
        return Offset - vertex.w;
    }

    /// <summary>
    /// Returns a vertex corresponding to a crossection between hyperplane and a line.
    /// Should be used when assured the point exists.
    /// </summary>
    /// <param name="a">4D point</param>
    /// <param name="b">4D point</param>
    /// <returns>3D point in the hyperplanes coordinates</returns>
    public Vector3 CrossSectionWithLine(Vector4 a, Vector4 b, float positionW)
    {
        float delta = (Offset - positionW - a[3]) / (b[3] - a[3]);
        float newX = a[0] + (b[0] - a[0]) * delta;
        float newY = a[1] + (b[1] - a[1]) * delta;
        float newZ = a[2] + (b[2] - a[2]) * delta;
        return new Vector3(newX, newY, newZ);
    }

    /// <summary>
    /// Returns cross-section with hyperplane
    /// </summary>
    /// <param name="vertices"></param>
    /// <param name="hyperplane"></param>
    /// <returns></returns>
    public Vector3[] CutEdges((int,int)[] FacetEdges, Vector4[] vertices, int[] FacetVertIndexes, float positionW)
    {

        List<Vector3> resultingPolygon = new List<Vector3>();

        foreach (var edge in FacetEdges)
        {
            var bod1 = vertices[FacetVertIndexes[edge.Item1]];
            var bod2 = vertices[FacetVertIndexes[edge.Item2]];
            var distance1 = DistanceTo(vertices[FacetVertIndexes[edge.Item1]]) - positionW;
            var distance2 = DistanceTo(vertices[FacetVertIndexes[edge.Item2]]) - positionW;


            if ((distance1 < 0 && distance2 >= 0)
                || (distance1 >= 0 && distance2 < 0))
            {
                resultingPolygon.Add(CrossSectionWithLine(vertices[FacetVertIndexes[edge.Item1]], vertices[FacetVertIndexes[edge.Item2]], positionW));
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
}