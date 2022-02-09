using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a 4D scene, handles objects, and physics
/// </summary>
public class Space4DManager : MonoBehaviour
{
    [SerializeField]
    public Hyperplane Hyperplane;

    List<Object4D> Objects;

    // Start is called before the first frame update
    void Start()
    {
        Objects = GetComponentsInChildren<Object4D>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
