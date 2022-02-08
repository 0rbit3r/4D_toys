using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class Hyperplane : MonoBehaviour
{
    float d = 0F;


    Slider Slider;

    // Start is called before the first frame update
    void Start()
    {
        Slider = new Slider(this);
    }

    // Update is called once per frame
    void Update()
    {
        Slider.Update();
        d = Slider.Value;
    }

    public float DistanceTo(Vector4 vertex)
    {
        return vertex.w - d;
    }

    public Vector3 CrossSectionWithLine(Vector4 a, Vector4 b)
    {
        float delta = (d - a[3]) / (b[3] - a[3]);
        float newX = a[0] + (b[0] - a[0]) * delta;
        float newY = a[1] + (b[1] - a[1]) * delta;
        float newZ = a[2] + (b[2] - a[2]) * delta;
        return new Vector3(newX, newY, newZ);
    }
}

class Slider
{
    public float Value = 0;

    GameObject RightController;
    GameObject LeftController;

    GameObject Cylinder;
    GameObject Ball;

    public float MaxValue = 2;

    public float SliderLength = 0.4f;

    private InputActionMap LeftControllerInputMap;
    private InputActionMap RightControllerInputMap;

    public Slider(Hyperplane h)
    {

        RightController = GameObject.Find("RightHand Controller");
        LeftController = GameObject.Find("LeftHand Controller");

        GameObject XrRig = GameObject.Find("XR Rig");

        LeftControllerInputMap = XrRig.GetComponent<ControllerInputReader>().LeftControllerInputMap;
        RightControllerInputMap = XrRig.GetComponent<ControllerInputReader>().RightControllerInputMap;

        Cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Cylinder.GetComponent<CapsuleCollider>().enabled = false;
        Cylinder.transform.SetParent(h.transform);
        Cylinder.transform.localPosition = new Vector3(0, 0, 0);
        Cylinder.transform.localScale = new Vector3(SliderLength/20, SliderLength, SliderLength/20);
        Cylinder.transform.Rotate(new Vector3(0, 0, 90));

        Cylinder.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Slider");


        Ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Ball.transform.SetParent(h.transform);
        Ball.transform.localPosition = new Vector3(0, 0, 0); 
        Ball.transform.localScale = new Vector3(SliderLength / 4, SliderLength / 4, SliderLength / 4);

        Ball.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Slider");
        
        //Ball.AddComponent<XRGrabInteractable>();
        //Ball.GetComponent<XRGrabInteractable>().tightenPosition = 0.4f;
        //Ball.GetComponent<Rigidbody>().useGravity = false;

    }
    
    public void Update()
    {
        CheckForPlayerInteraction();
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        float posX = Ball.transform.localPosition.x;
        if (posX < -SliderLength)
        {
            posX = -SliderLength;
        }
        if (posX > SliderLength)
        {
            posX = SliderLength;
        }
        Ball.transform.localPosition = new Vector3(posX, 0, 0);

        Value = (float)(posX / (SliderLength / MaxValue));
    }
    
    public void CheckForPlayerInteraction()
    {

        if (LeftControllerInputMap.FindAction("Select").ReadValue<float>() > 0.5 && Math.Abs((LeftController.transform.position - Ball.transform.position).magnitude) < 0.07)
        {
            Ball.transform.position = new Vector3(LeftController.transform.position.x, Ball.transform.position.y, Ball.transform.position.z);
        }

        if (RightControllerInputMap.FindAction("Select").ReadValue<float>() > 0.5 && Math.Abs((RightController.transform.position - Ball.transform.position).magnitude) < 0.07)
        {
            Ball.transform.position = new Vector3(RightController.transform.position.x, Ball.transform.position.y, Ball.transform.position.z);
        }
    }
}