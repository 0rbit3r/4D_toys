using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Object4D;

/// <summary>
/// Spawns a 4D object when griipped by user
/// </summary>
public class Object4DSpawner : MonoBehaviour
{
    [SerializeField]
    Shapes Shape;

    [SerializeField]
    bool ManualInsertion;

    [SerializeField]
    ControllerInputReader InputReader;

    [SerializeField]
    Physics4DSpace Space;

    GameObject RightController;
    GameObject LeftController;

    int AntiSpamTimer;
    readonly int AntiSpamMaxValue = 20;

    // Start is called before the first frame update
    void Start()
    {
        AntiSpamTimer = AntiSpamMaxValue;
        RightController = GameObject.Find("RightHand Controller");
        LeftController = GameObject.Find("LeftHand Controller");
    }

    // Update is called once per frame
    void Update()
    {
        if (AntiSpamTimer != 0)
        {
            AntiSpamTimer--;
            return;
        }


        if ((InputReader.LeftControllerInputMap.FindAction("Select").ReadValue<float>() > 0.5 &&
            Math.Abs((LeftController.transform.position - transform.position).magnitude) < 0.07)
            || (InputReader.RightControllerInputMap.FindAction("Select").ReadValue<float>() > 0.5 &&
            Math.Abs((RightController.transform.position - transform.position).magnitude) < 0.07)
            || ManualInsertion)
        {
            AntiSpamTimer = AntiSpamMaxValue;
            ManualInsertion = false;

            var newObject = new GameObject();
            newObject.transform.SetParent(Space.transform);
            newObject.transform.localPosition = new Vector3(0.015f, 1.13f, 0.73f);
            newObject.AddComponent<Object4D>();
            newObject.AddComponent<RigidBody4D>();
            newObject.GetComponent<Object4D>().Shape = Shape;
        }
    }
}
