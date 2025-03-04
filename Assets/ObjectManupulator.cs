using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{
    public Camera playerCamera;  // Assign the player camera in the inspector
    public float pickUpRange = 3f;  // Distance to pick up objects
    public Transform holdPosition;  // Empty GameObject for holding the object
    public float rotationSpeed = 100f;
    public float scaleSpeed = 0.1f;
    public float holdDistanceStep = 0.2f;  // Step size for moving the Hold Position
    public float holdHeightSensitivity = 0.05f; // Sensitivity for vertical movement
    public float minHoldHeight = -1f;  // Min height relative to the player
    public float maxHoldHeight = 2f;  // Max height relative to the player

    private GameObject heldObject;
    private Rigidbody heldObjectRb;
    public float currentHoldHeight = -0.4f;  // Track hold position height

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Left-click to pick up/drop
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        if (heldObject != null)  // Changed condition to check if heldObject is not null
        {
            HandleRotation();
            HandleScaling();
            HandleHoldPosition();
        }
    }

    void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("Interactable"))  // Ensures object has correct tag
            {
                heldObject = hit.collider.gameObject;
                heldObjectRb = heldObject.GetComponent<Rigidbody>();

                if (heldObjectRb)
                {
                    heldObjectRb.useGravity = false;
                    heldObjectRb.isKinematic = true;
                }

                heldObject.transform.position = holdPosition.position;
                heldObject.transform.parent = holdPosition;
            }
        }
    }

    void DropObject()
    {
        if (heldObjectRb)
        {
            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
        }

        heldObject.transform.parent = null;
        heldObject = null;
        heldObjectRb = null;
    }

    void HandleRotation()
    {
        float rotateX = 0f;
        float rotateY = 0f;

        // Rotate around the X-axis (Q / E)
        if (Input.GetKey(KeyCode.Q)) rotateX = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) rotateX = rotationSpeed * Time.deltaTime;

        // Rotate around the Y-axis (R / T)
        if (Input.GetKey(KeyCode.R)) rotateY = -rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.T)) rotateY = rotationSpeed * Time.deltaTime;

        // Apply rotation
        heldObject.transform.Rotate(rotateX, rotateY, 0, Space.World);
    }

    void HandleScaling()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            heldObject.transform.localScale += Vector3.one * scroll * scaleSpeed;
        }
    }

void HandleHoldPosition()
{
    // Move forward/backward with Up/Down arrow keys
    if (Input.GetKey(KeyCode.UpArrow))
    {
        holdPosition.position += playerCamera.transform.forward * holdDistanceStep * Time.deltaTime;
    }
    else if (Input.GetKey(KeyCode.DownArrow))
    {
        holdPosition.position -= playerCamera.transform.forward * holdDistanceStep * Time.deltaTime;
    }

    // Adjust height using Mouse Y movement
    float mouseY = Input.GetAxis("Mouse Y");
    currentHoldHeight += mouseY * holdHeightSensitivity;  // Invert Mouse Y axis

    // Clamp height to stay within defined limits
    currentHoldHeight = Mathf.Clamp(currentHoldHeight, minHoldHeight, maxHoldHeight);

    // Apply the height adjustment to the hold position
    Vector3 newPosition = holdPosition.position;
    newPosition.y = playerCamera.transform.position.y + currentHoldHeight;
    holdPosition.position = newPosition;
}

}
