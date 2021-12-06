using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Object4D : MonoBehaviour
{
    [SerializeField]
    string FilePath;
    Vector4[] Vertices;
    Facet[] Facets;

    [SerializeField]
    Hyperplane Hyperplane;

    Mesh Mesh;

    public void Start()
    {
        FromFile(FilePath);

        Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = Mesh;
    }

    public void Update()
    {
        RenderIn(Hyperplane);
    }

    public void FromFile(string filePath)
    {
        try
        {
            StreamReader reader = new StreamReader(filePath);

            Vertices = new Vector4[int.Parse(reader.ReadLine())];
            Facets = new Facet[int.Parse(reader.ReadLine())];

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
                while ((line = reader.ReadLine()).Length != 0)
                {
                    facetvertices.Add(int.Parse(line));
                }
                Facets[i] = new Facet(facetvertices.ToArray(), this);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"File Format Error ({e.Message})");
        }
    }

    public void RenderIn(Hyperplane plane)
    {
        var vectors = Facets[2].CutWith(plane);

        //foreach (var facet in Facets)
        //{
        //    facet.CutWith(plane);
        //}

        UpdateMesh(vectors);
    }

    public class Facet
    {
        private Object4D ParentObject;

        public int[] FacetVerts;

        public int[][] Edges;
        public Facet(int[] vertices, Object4D parentObject)
        {
            FacetVerts = vertices;
            ParentObject = parentObject;

            CreateEdges();
        }

        private void CreateEdges()
        {
            Vector4[] vertices = GetDereferencedVertexList();
            Vector3[] flattenedVertices = Utils4D.ProjectTo3D(vertices);


        }

        public Vector3[] CutWith(Hyperplane plane)
        {
            List<Vector3> result = new List<Vector3>();

            List<int> above = new List<int>();
            List<int> below = new List<int>();

            for (int i = 0; i < FacetVerts.Length; i++)
            {
                if (plane.DistanceTo(ParentObject.Vertices[FacetVerts[i]]) <= 0)
                {
                    below.Add(FacetVerts[i]);
                }
                else
                {
                    above.Add(FacetVerts[i]);
                }
            }

            foreach (var aboveVert in above)
            {
                foreach (var belowVert in below)
                {
                    result.Add(plane.CrossSectionWithLine(ParentObject.Vertices[belowVert], ParentObject.Vertices[aboveVert]));
                }
            }

            return result.ToArray();
        }

        public Vector4[] GetDereferencedVertexList()
        {
            var toReturn = new Vector4[FacetVerts.Length];

            for (int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = ParentObject.Vertices[FacetVerts[i]];
            }

            return toReturn;
        }
    }

    private void UpdateMesh(Vector3[] vertices)
    {
        Mesh.Clear();

        Mesh.vertices = vertices;

        var triangles = new List<int>();
        if (vertices.Length > 2)
            for (int i = 2; i < Vertices.Length; i++)
            {
                triangles.Add(0);
                triangles.Add(i - 1);
                triangles.Add(i);

                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i - 1);
            }

        Mesh.triangles = triangles.ToArray();
    }


}
