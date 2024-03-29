﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    /// <summary>
    /// class spawning and controlling slider, that can be handled in VR to change hyperplane offset.
    /// </summary>
    class Slider
    {
        public float Value = 0;

        GameObject RightController;
        GameObject LeftController;

        GameObject Cylinder;
        GameObject Ball;

        public float MaxValue = 1;

        public float SliderLength = 0.4f;

        private InputActionMap LeftControllerInputMap;
        private InputActionMap RightControllerInputMap;

        public Slider(ViewerHyperplane h)
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
            Cylinder.transform.localScale = new Vector3(SliderLength / 20, SliderLength, SliderLength / 20);
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
}
