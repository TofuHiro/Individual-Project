using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("The head transform of the player")]
    [SerializeField] Transform head;
    [Tooltip("The orientation transform of the player")]
    [SerializeField] Transform orientation;
    [Tooltip("The minimum vertical rotation value the player can look")]
    [SerializeField] float minClamp = -89f;
    [Tooltip("The maximum vertical rotation value the player can look")]
    [SerializeField] float maxClamp = 89f;

    [Header("Mouse Settings")]
    [Tooltip("The multiplier applied to vertical mouse inputs to rotate the player")]
    [SerializeField] float verticalSensitivity = 10f;
    [Tooltip("The multiplier applied to horizontal mouse inputs to rotate the player")]
    [SerializeField] float horizontalSensitivity = 10f;

    Quaternion rotationDir;
    float headRotation = 0f, bodyRotation = 0f;

    public Quaternion GetRotation()
    {
        return orientation.rotation;
    }

    /// <summary>
    /// Set a direction for the player to look towards
    /// </summary>
    /// <param name="_rotDir">The 2D axis to rotate towards</param>
    public void SetRotationDir(Quaternion _rotDir)
    {
        rotationDir = _rotDir;
    }

    /// <summary>
    /// Set a rotation
    /// </summary>
    /// <param name="_rot"></param>
    public void SetRotation(Quaternion _rot)
    {
        head.rotation = Quaternion.Euler(_rot.y, _rot.x, 0f);
        orientation.rotation = Quaternion.Euler(0f, _rot.x, 0f);
    }

    void Update()
    {
        Rotation();
    }

    /// <summary>
    /// Rotates the player camera 
    /// </summary>
    void Rotation()
    {
        if (rotationDir.eulerAngles.magnitude > 0) {
            bodyRotation += rotationDir.x * 150f * Time.deltaTime * horizontalSensitivity;

            //Clamp head rotation
            headRotation -= rotationDir.y * 150f * Time.deltaTime * verticalSensitivity;
            headRotation = Mathf.Clamp(headRotation, minClamp, maxClamp);

            head.rotation = Quaternion.Euler(headRotation, bodyRotation, 0f);
            orientation.rotation = Quaternion.Euler(0f, bodyRotation, 0f);
        }
    }
}
