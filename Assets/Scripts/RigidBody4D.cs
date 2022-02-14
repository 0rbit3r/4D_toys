using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Object4D))]
public class RigidBody4D : MonoBehaviour
{
    public Object4D Object { get; private set; }

    public IBoundingBox BoundingBox { get; private set; }

    public Vector4 Momentum { get; private set; }

    public Matrix4x4 RotationalMomentum { get; private set; }

    ControllerInputReader InputReader;

    GameObject RightController;
    GameObject LeftController;

    Vector3 oldRotation;
    readonly float rotationSpeed = 0.05f;

    bool ManipulatedLastFrame = false;

    static RigidBody4D CurrentlyHeldObject;

    // Start is called before the first frame update
    void Start()
    {
        Object = gameObject.GetComponent<Object4D>();
        BoundingBox = Object.BoundingBox;

        RightController = GameObject.Find("RightHand Controller");
        LeftController = GameObject.Find("LeftHand Controller");

        InputReader = GameObject.Find("XR Rig").GetComponent<ControllerInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BoundingBox == null)
        {
            BoundingBox = Object.BoundingBox;
        }

        HandleHandling();
    }

    /// <summary>
    /// Handles player interaction with the Rigid body
    /// </summary>
    void HandleHandling()
    {
        var isHeldThisFrame = false;

        if (Object.IsKinematic && (CurrentlyHeldObject == null || CurrentlyHeldObject == this))
        {
            Transform controllerTransform = null;

            var hyperplane = transform.parent.GetComponent<Physics4DSpace>().Hyperplane;
            if (InputReader.LeftControllerInputMap.FindAction("Select").ReadValue<float>() > 0.5 &&
                Math.Abs((LeftController.transform.position - transform.position).magnitude) < 0.07)
            {
                isHeldThisFrame = true;
                controllerTransform = LeftController.transform;
            }

            if (InputReader.RightControllerInputMap.FindAction("Select").ReadValue<float>() > 0.5 &&
                Math.Abs((RightController.transform.position - transform.position).magnitude) < 0.07)
            {
                isHeldThisFrame = true;
                controllerTransform = RightController.transform;
            }

            if (isHeldThisFrame)
            {
                CurrentlyHeldObject = this;
                var newRotation = controllerTransform.rotation.eulerAngles;
                newRotation = new Vector3(newRotation.x / 180 * (float)Math.PI, newRotation.y / 180 * (float)Math.PI, newRotation.z / 180 * (float)Math.PI);

                if (ManipulatedLastFrame)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.xy, oldRotation.z - newRotation.z);
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.zx, newRotation.y - oldRotation.y);
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.yz, oldRotation.x - newRotation.x);
                }

                ManipulatedLastFrame = true;
                oldRotation = newRotation;

                Object.MoveTo(new Vector4(controllerTransform.position.x, controllerTransform.position.y, controllerTransform.position.z, hyperplane.Offset));

                var xwRotationInput = InputReader.RightControllerInputMap.FindAction("Turn").ReadValue<Vector2>().x;
                var ywRotationInput = InputReader.RightControllerInputMap.FindAction("Turn").ReadValue<Vector2>().y;
                var zwRotationInput = InputReader.LeftControllerInputMap.FindAction("Turn").ReadValue<Vector2>().y;


                if (xwRotationInput > 0.5)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.xw, rotationSpeed);
                }
                if (xwRotationInput < -0.5)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.xw, -rotationSpeed);
                }

                if (ywRotationInput > 0.5)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.yw, rotationSpeed);
                }
                if (ywRotationInput < -0)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.yw, -rotationSpeed);
                }

                if (zwRotationInput > 0.5)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.zw, rotationSpeed);
                }
                if (zwRotationInput < -0.5)
                {
                    Object.SimplyRotate(Assets.Scripts.RotationPlane.zw, -rotationSpeed);
                }
            }
            else
            {
                ManipulatedLastFrame = false;
                CurrentlyHeldObject = null;
            }
        }
    }
}