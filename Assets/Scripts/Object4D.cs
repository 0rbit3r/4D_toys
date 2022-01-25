using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Assets.Scripts;

public class Object4D : MonoBehaviour
{

    [SerializeField]
    string FilePath;

    [SerializeField]
    Hyperplane Hyperplane;

    [SerializeField]
    bool DebugRotate4D_XY;
    [SerializeField]
    bool DebugRotate4D_XZ;
    [SerializeField]
    bool DebugRotate4D_XW;
    [SerializeField]
    bool DebugRotate4D_YZ;
    [SerializeField]
    bool DebugRotate4D_YW;
    [SerializeField]
    bool DebugRotate4D_ZW;

    public bool RotationNeedsUpdate;

    public Vector4[] Vertices;

    public Vector4[] RotatedVertices;

    GameObject[] Facets;

    Vector4 Position;
    Matrix4x4 RotationMatrix = Matrix4x4.identity;


    public void Start()
    {
        FromFile(FilePath);
        UpdateRotation();
        //Position = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0);
        //Rotation = new Vector4(transform.rotation.x)
    }

    public void Update()
    {
        if (DebugRotate4D_XY)
        {
            SimplyRotate(RotationPlane.xy, 0.005);
        }
        if (DebugRotate4D_XZ)
        {
            SimplyRotate(RotationPlane.xz, 0.005);
        }
        if (DebugRotate4D_XW)
        {
            SimplyRotate(RotationPlane.xw, 0.005);
        }
        if (DebugRotate4D_YZ)
        {
            SimplyRotate(RotationPlane.yz, 0.005);
        }
        if (DebugRotate4D_YW)
        {
            SimplyRotate(RotationPlane.yw, 0.005);
        }
        if (DebugRotate4D_ZW)
        {
            SimplyRotate(RotationPlane.zw, 0.005);
        }

        if (RotationNeedsUpdate)
        {
            UpdateRotation();
        }

        foreach (var facet in Facets)
        {
            facet.GetComponent<Facet>().RenderIn(Hyperplane);
        }
    }

    public void FromFile(string filePath)
    {
        try
        {
            StreamReader reader = new StreamReader(filePath);

            Vertices = new Vector4[int.Parse(reader.ReadLine())];
            RotatedVertices = new Vector4[Vertices.Length];
            Facets = new GameObject[int.Parse(reader.ReadLine())];

            reader.ReadLine();

            for (int i = 0; i < Vertices.Length; i++)
            {
                float[] coors = Array.ConvertAll(reader.ReadLine().Split(), x => float.Parse(x));
                Vertices[i] = new Vector4(coors[0], coors[1], coors[2], coors[3]);
            }

            reader.ReadLine();

            for (int i = 0; i < Facets.Length; i++)
            {
                List<int> facetvertices = new List<int>();

                string line;
                while ((line = reader.ReadLine()) != null && line.Length != 0)
                {
                    facetvertices.Add(int.Parse(line));
                }
                Facets[i] = new GameObject();
                Facets[i].name = Utils4D.FacetColors[i % Utils4D.FacetColors.Length].ToString();
                Facets[i].AddComponent<Facet>();
                Facets[i].GetComponent<Facet>().Init(facetvertices.ToArray(), Utils4D.FacetColors[i % Utils4D.FacetColors.Length], this);
                Facets[i].transform.SetParent(gameObject.transform);
                Facets[i].transform.localPosition = new Vector3(0, 0, 0);
                Facets[i].transform.localScale = new Vector3(1, 1, 1);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("File not parsed!");
            throw new Exception($"File Format Error ({e.Message})");
        }
    }

    public void SimplyRotate(RotationPlane plane, double angle)
    {
        RotationNeedsUpdate = true;


        Matrix4x4 newRotation = new Matrix4x4();
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);

        switch (plane)
        {
            case RotationPlane.xy:
                newRotation = new Matrix4x4(new Vector4(cos, -sin, 0, 0),
                                            new Vector4(sin,  cos, 0, 0),
                                            new Vector4(0, 0, 1, 0),
                                            new Vector4(0, 0, 0, 1));
                break;
            case RotationPlane.xz:
                newRotation = new Matrix4x4(new Vector4(cos, 0, -sin, 0),
                                            new Vector4(0, 1, 0, 0),
                                            new Vector4(sin, 0, cos, 0),
                                            new Vector4(0, 0, 0, 1));
                break;
            case RotationPlane.xw:
                newRotation = new Matrix4x4(new Vector4(cos, 0, 0, -sin),
                                            new Vector4(0, 1, 0, 0),
                                            new Vector4(0, 0, 1, 0),
                                            new Vector4(sin, 0, 0, cos));
                break;
            case RotationPlane.yz:
                newRotation = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                            new Vector4(0, cos, -sin, 0),
                                            new Vector4(0, sin, cos, 0),
                                            new Vector4(0, 0, 0, 1));
                break;
            case RotationPlane.yw:
                newRotation = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                            new Vector4(0, cos, 0, -sin),
                                            new Vector4(0, 0, 1, 0),
                                            new Vector4(0, sin, 0, cos));
                break;
            case RotationPlane.zw:
                newRotation = new Matrix4x4(new Vector4(1, 0, 0, 0),
                                            new Vector4(0, 1, 0, 0),
                                            new Vector4(0, 0, cos, -sin),
                                            new Vector4(0, 0, sin, cos));
                break;
        }


        RotationMatrix = newRotation * RotationMatrix;
    }

    public void UpdateRotation()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            RotatedVertices[i] = RotationMatrix * Vertices[i];
        }
    }
}
