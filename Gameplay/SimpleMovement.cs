using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    private enum Space
    {
        Global,
        Local
    }

    [SerializeField] private Space _movementSpace = Space.Local;
    public Vector3 MovementPerSecond;

    private void Update()
    {
        switch (_movementSpace)
        {
            case Space.Global:
                transform.position += MovementPerSecond * Time.deltaTime;
                break;
            case Space.Local:
                Vector3 right = transform.right * MovementPerSecond.x;
                Vector3 up = transform.up * MovementPerSecond.y;
                Vector3 forward = transform.forward * MovementPerSecond.z;
                Vector3 movement = right + up + forward;

                transform.position += movement * Time.deltaTime;
                break;
            default:
                break;
        }
    }
}
