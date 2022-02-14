using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a 4D scene, handles objects, and physics
/// </summary>
public class Physics4DSpace : MonoBehaviour
{
    [SerializeField]
    public ViewerHyperplane Hyperplane;

    [SerializeField]
    public Cage4D Cage;

    RigidBody4D[] RigidBodies;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMomentsFromCollisions();
    }

    void UpdateMomentsFromCollisions()
    {
        if(RigidBodies == null)
        {
            RigidBodies = GetComponentsInChildren<RigidBody4D>().ToArray();
        }

        for (int i = 0; i < RigidBodies.Length - 1; i++)
        {
            for (int j = i + 1; j < RigidBodies.Length; j++)
            {
                if (RigidBodies[i].BoundingBox.Intersects(RigidBodies[j].BoundingBox))
                {
                    Vector4 collisionVector = GetObjectCollisionVector(RigidBodies[i],RigidBodies[j]);
                }
            }
        }
    }

    Facet[][] GetBoundingBoxTouchingFacets(RigidBody4D o1, RigidBody4D o2)
    {
        List<Facet> facets1 = new List<Facet>();
        List<Facet> facets2 = new List<Facet>();

        for (int i = 0; i < o1.Object.Facets.Length; i++)
        {
            for (int j = 0; j < o2.Object.Facets.Length; j++)
            {
                var f1 = o1.Object.Facets[i].GetComponent<Facet>();
                var f2 = o2.Object.Facets[i].GetComponent<Facet>();
                if (f1.BoundingBox.Intersects(f2.BoundingBox))
                {
                    facets1.Add(f1);
                    facets2.Add(f2);
                }
            }
        }
        return new Facet[][] {facets1.Distinct().ToArray(), facets2.Distinct().ToArray() };
    }

    Vector4 GetObjectCollisionVector(RigidBody4D o1, RigidBody4D o2)
    {
        var touchingFacets = GetBoundingBoxTouchingFacets(o1, o2);

        List<Vector4> facetCollisionVectors = new List<Vector4>();

        for (int i = 0; i < o1.Object.Facets.Length; i++)
        {
            for (int j = 0; j < o2.Object.Facets.Length; j++)
            {
                facetCollisionVectors.Add(GetFacetCollisionVector(touchingFacets[0][i], touchingFacets[j][j]));
            }
        }

        return new Vector4();
    }

    Vector4 GetFacetCollisionVector(Facet f1, Facet f2)
    {

        return new Vector4(); 
    }
}
