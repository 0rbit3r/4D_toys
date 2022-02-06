using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInputReader : MonoBehaviour
{
    public InputActionMap LeftControllerInputMap { get; private set; }
    public InputActionMap RightControllerInputMap { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        LeftControllerInputMap = GetComponent<PlayerInput>().actions.FindActionMap("XRI LeftHand");
        RightControllerInputMap = GetComponent<PlayerInput>().actions.FindActionMap("XRI RightHand");
        LeftControllerInputMap.Enable();
        RightControllerInputMap.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
