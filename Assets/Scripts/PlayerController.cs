using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(h) < 0.01f && Mathf.Abs(v) < 0.01f) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = camForward.normalized * v + camRight.normalized * h;

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        // ѕоворачиваем игрока в сторону движени€
        transform.rotation = Quaternion.LookRotation(moveDir);
    }
}
