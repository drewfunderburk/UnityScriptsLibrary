using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCameraControllerQuaternions : MonoBehaviour
{
    public float MouseSensitivity = 100;
    public bool InvertY = false;
    [Space]
    [Tooltip("Rigidbody to rotate with the object")]
    public Rigidbody ShipRigidbody;
    [Tooltip("How fast to lerp the rigidbody towards the look vector. (Lower is slower)")]
    public float LerpSpeed = 5;

    private void Start()
    {
        // Lock the cursor into the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Get the change in mouse position
        Vector2 deltaMouse = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));
        if (InvertY) deltaMouse.y *= -1;

        Quaternion rotationX = Quaternion.AngleAxis(deltaMouse.x, Vector3.up);
        Quaternion rotationY = Quaternion.AngleAxis(deltaMouse.y, Vector3.right);

        Quaternion rotation = transform.rotation * rotationX * rotationY;
        transform.rotation = rotation;


        // Lerp the ship's current rotation towards this object's rotation
        Quaternion newShipRotation = Quaternion.Lerp(ShipRigidbody.rotation, transform.rotation, LerpSpeed * Time.deltaTime);

        // Apply ship rotation
        ShipRigidbody.MoveRotation(newShipRotation);
    }
}
