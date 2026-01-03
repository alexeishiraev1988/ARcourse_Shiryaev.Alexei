using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private HingeJoint hinge;
    private JointMotor motor;
    private bool isOpen = false;

    public float motorSpeed = 100f; 
    public float motorForce = 1000f; 

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useMotor = true; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;

            motor = hinge.motor;

            motor.targetVelocity = isOpen ? motorSpeed : -motorSpeed;
            motor.force = motorForce;
            motor.freeSpin = false;

            hinge.motor = motor;
            hinge.useMotor = true;
        }
    }
}
