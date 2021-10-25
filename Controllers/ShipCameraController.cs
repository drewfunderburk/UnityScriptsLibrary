using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCameraController : MonoBehaviour
{
    public float MouseSensitivity = 100;
    public bool InvertMouseY = false;
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

        // Invert Y
        if (InvertMouseY) deltaMouse.y *= -1;

        // Calculate new rotation
        Vector3 newRotation = new Vector3(deltaMouse.y, deltaMouse.x, 0);
        Vector3 rotation = transform.eulerAngles + newRotation;

        // Apply mouse rotation to this object
        transform.eulerAngles = rotation;


        // Lerp the ship's current rotation towards this object's rotation
        Quaternion newShipRotation = Quaternion.Lerp(ShipRigidbody.rotation, transform.rotation, LerpSpeed * Time.deltaTime);

        // Apply ship rotation
        ShipRigidbody.MoveRotation(newShipRotation);
    }
}
