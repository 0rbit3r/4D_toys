using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hyperplane : MonoBehaviour
{
    //[SerializeField]
    //string CurrentEquation;
    //[SerializeField]
    //[Range(-1, 1)]
    //float a = 0;
    //[SerializeField]
    //[Range(-1, 1)]
    //float b = 0;
    //[SerializeField]
    //[Range(-1, 1)]
    //float c = 0;
    [SerializeField]
    [Range(-1, 1)]
    float d = 0.5F;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //UpdateEquation();
    }

    public float DistanceTo(Vector4 vertex)
    {
        return vertex[3] - d;
    }

    public Vector3 CrossSectionWithLine(Vector4 a, Vector4 b)
    {
        float delta = (d - a[3]) / (b[3] - a[3]);
        float newX = a[0] + (b[0] - a[0]) * delta;
        float newY = a[1] + (b[1] - a[1]) * delta;
        float newZ = a[2] + (b[2] - a[2]) * delta;
        return new Vector3(newX, newY, newZ);
    }

    //void UpdateEquation()
    //{
    //    string a_str = a < 0 ? $"- {-a}" : a.ToString("F2");
    //    string b_str = b < 0 ? $"- {-b}" : "+ " + b.ToString("F2");
    //    string c_str = c < 0 ? $"- {-c}" : "+ " + c.ToString("F2");
    //    string d_str = d < 0 ? $"- {-d}" : "+ " + d.ToString("F2");
    //    CurrentEquation = $"w = {a_str}*x {b_str}*y {c_str}*z {d_str}";
    //}
}
