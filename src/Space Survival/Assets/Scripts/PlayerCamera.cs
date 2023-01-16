using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("The head transform of the player")]
    [SerializeField] Transform head;
    [Tooltip("The orientation transform of the player")]
    [SerializeField] Transform orientation;

    [Header("Mouse Settings")]
    [Tooltip("The multiplier applied to vertical mouse inputs to rotate the player")]
    [SerializeField] float verticalSensitivity = 10f;
    [Tooltip("The multiplier applied to horizontal mouse inputs to rotate the player")]
    [SerializeField] float horizontalSensitivity = 10f;

    Vector2 rotationDir;
    float headRotation = 0f, bodyRotation = 0f;

    public void SetRotation(Vector2 _rotDir)
    {
        rotationDir = _rotDir;
    }

    void Update()
    {
        Rotation();
    }

    void Rotation()
    {
        if (rotationDir.magnitude > 0) {
            bodyRotation += rotationDir.x * Time.deltaTime * horizontalSensitivity;

            //Clamp head rotation
            headRotation -= rotationDir.y * Time.deltaTime * verticalSensitivity;
            headRotation = Mathf.Clamp(headRotation, -89f, 89f);

            head.rotation = Quaternion.Euler(headRotation, bodyRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, bodyRotation, 0f);
        }
    }
}
