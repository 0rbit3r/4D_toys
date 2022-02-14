using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Assets.Scripts;

public class Object4D : MonoBehaviour
{
    public enum Shapes
    {
        Tesseract,
        Cell_5,
        Spheric_Icosahedron,
        Test,
    }
    [SerializeField]
    public Shapes Shape;

    [SerializeField]
    public bool IsKinematic = true;

    [SerializeField]
    public float InitialScale = 0.2f;

    //Following serializedFields are only for debug purposes.
    [SerializeField]
    bool Rotate4D_XY;
    [SerializeField]
    bool Rotate4D_XZ;
    [SerializeField]
    bool Rotate4D_XW;
    [SerializeField]
    bool Rotate4D_YZ;
    [SerializeField]
    bool Rotate4D_YW;
    [SerializeField]
    bool Rotate4D_ZW;
    [SerializeField]
    float Degrees = -1;

    private bool RotationNeedsUpdate;
    private bool PositionNeedsUpdate;

    public Vector4[] Vertices;

    public Vector4[] RotatedVertices;

    public GameObject[] Facets { get; private set; }

    public Vector4 Position { get; private set; }
    //Vector4 Scale;
    Matrix4x4 RotationMatrix = Matrix4x4.identity;

    ViewerHyperplane Hyperplane;

    public IBoundingBox BoundingBox;

    public void Awake()
    {

    }

    public void Start()
    {
        Degrees = -1;
#if DEBUG
        FromFile(Shape);
#else
        FromString(ObjectDataManager.GetShapeData(Shape));
#endif

        Position = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0);
        //Scale = new Vector4(InitialScale, InitialScale, InitialScale, InitialScale);
        Vertices = Vertices.Select(v => new Vector4(v.x * InitialScale, v.y * InitialScale, v.z * InitialScale, v.w * InitialScale)).ToArray();

        if (gameObject.transform.parent == null)
        {
            Hyperplane = new ViewerHyperplane();
        }
        else
        {
            Hyperplane = gameObject.GetComponentInParent<Physics4DSpace>().Hyperplane;
        }
        BoundingBox = new ObjectBoundingBox(Position, Vertices);

        Update3DPosition();
        UpdateRotation();
    }
    public void Update()
    {

        if (Degrees == -1 || Degrees > 0)
        {
            if (Rotate4D_XY)
            {
                SimplyRotate(RotationPlane.xy, 0.005);
                if (Degrees != -1) Degrees -= 180f / (float)Math.PI * 0.005f;
            }
            if (Rotate4D_XZ)
            {
                SimplyRotate(RotationPlane.zx, 0.005);
                if (Degrees != -1) Degrees -= 180f / (float)Math.PI * 0.005f;
            }
            if (Rotate4D_XW)
            {
                SimplyRotate(RotationPlane.xw, 0.005);
                if (Degrees != -1) Degrees -= 180f / (float)Math.PI * 0.005f;
            }
            if (Rotate4D_YZ)
            {
                SimplyRotate(RotationPlane.yz, 0.005);
                if (Degrees != -1) Degrees -= 180f / (float)Math.PI * 0.005f;
            }
            if (Rotate4D_YW)
            {
                SimplyRotate(RotationPlane.yw, 0.005);
                if (Degrees != -1) Degrees -= 180f / (float)Math.PI * 0.005f;
            }
            if (Rotate4D_ZW)
            {
                SimplyRotate(RotationPlane.zw, 0.005);
                if (Degrees != -1) Degrees -= 180f / (float)Math.PI * 0.005f;
            }
            if (Degrees != -1 && Degrees < 0)
            {
                Degrees = 0;
            }
        }

        if (RotationNeedsUpdate)
        {
            UpdateRotation();
            UpdateBoundingBox();
        }

        if (PositionNeedsUpdate)
        {
            Update3DPosition();
            UpdateBoundingBox();
        }

        foreach (var facet in Facets)
        {
            facet.GetComponent<Facet>().RenderIn(Hyperplane, Position.w);
        }
    }

    public void Move(Vector4 direction)
    {
        Position += direction;

        PositionNeedsUpdate = true;
    }

    public void MoveTo(Vector4 newPosition)
    {
        Position = newPosition;

        PositionNeedsUpdate = true;
    }
    void Update3DPosition()
    {
        gameObject.transform.position = new Vector3(Position.x, Position.y, Position.z);
    }

    string ShapeToPath(Shapes shape)
    {
        return $"Assets/4D objects/{Enum.GetName(typeof(Shapes), shape)}.4dm";
    }

    public void FromFile(Shapes shape)
    {
        string filePath = ShapeToPath(shape);
        StreamReader reader = new StreamReader(filePath);
        FromReader(reader);

    }

    public void FromString(string str)
    {

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(str);
        writer.Flush();
        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        FromReader(reader);
    }

    public void FromReader(StreamReader reader)
    {
        try
        {

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

    /// <summary>
    /// Updates rotation matrix by multiplying it with rotation in one of 6 canonical rotation planes by given angle in radians.
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="angle">Angle in radians</param>
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
                                            new Vector4(sin, cos, 0, 0),
                                            new Vector4(0, 0, 1, 0),
                                            new Vector4(0, 0, 0, 1));
                break;
            case RotationPlane.zx:
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

    /// <summary>
    /// Updates vertices by multiplying them with rotatrion Matrix;
    /// </summary>
    private void UpdateRotation()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            RotatedVertices[i] = RotationMatrix * Vertices[i];
        }
        RotationNeedsUpdate = false;
    }

    private void UpdateBoundingBox()
    {
        BoundingBox.Update(Position, Vertices);
    }
}
