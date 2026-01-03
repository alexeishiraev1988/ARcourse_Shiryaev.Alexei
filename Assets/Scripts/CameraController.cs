using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -6);
    [SerializeField] private float mouseSensitivity = 3f;

    private float yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
    }

    void LateUpdate()
    {
        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target);
    }
}
