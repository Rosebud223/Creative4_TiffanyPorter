using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GET_FirstPersonController : MonoBehaviour
{
    public float WalkSpeed = 5f;
    public float SprintMultiplier = 2f;
    public float JumpForce = 5f;
    public float GroundCheckDistance = 1.5f;
    public float LookSensitivityX = 1f;
    public float LookSensitivityY = 1f;
    public float MinYLookAngle = -90f;
    public float MaxYLookAngle = 90f;
    public Transform PlayerCamera;
    public float Gravity = -9.81f;

    private Vector3 velocity;
    private float verticalRotation = 0;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleCameraRotation();
    }

    private void HandleMovement()
    {
        float horizontalMovement = 0f;
        float verticalMovement = 0f;

        if (Input.GetKey(KeyCode.W)) verticalMovement = 1f;
        if (Input.GetKey(KeyCode.S)) verticalMovement = -1f;
        if (Input.GetKey(KeyCode.A)) horizontalMovement = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalMovement = 1f;

        Vector3 moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
        moveDirection.Normalize();

        float speed = WalkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= SprintMultiplier;
        }

        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            velocity.y = Mathf.Sqrt(JumpForce * -2f * Gravity);
        }

        velocity.y += Gravity * Time.deltaTime;
        characterController.Move((moveDirection * speed + velocity) * Time.deltaTime);
    }

    private void HandleCameraRotation()
    {
        if (PlayerCamera != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * LookSensitivityX;
            float mouseY = Input.GetAxis("Mouse Y") * LookSensitivityY;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, MinYLookAngle, MaxYLookAngle);

            PlayerCamera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, GroundCheckDistance);
    }
}
