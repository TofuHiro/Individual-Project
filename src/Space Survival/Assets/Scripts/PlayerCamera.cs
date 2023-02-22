using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public bool CanRotate { get; private set; } = true;

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

    void OnEnable()
    {
        PlayerInventory.OnInventoryOpen += DisableRotation;
        PlayerInventory.OnInventoryClose += EnableRotation;
    }

    void OnDisable()
    {
        PlayerInventory.OnInventoryOpen -= DisableRotation;
        PlayerInventory.OnInventoryClose -= EnableRotation;
    }

    /// <summary>
    /// Set a direction for the player to look towards
    /// </summary>
    /// <param name="_rotDir">The 2D axis to rotate towards</param>
    public void SetRotation(Vector2 _rotDir)
    {
        rotationDir = _rotDir;
    }

    /// <summary>
    /// Toggles the player's ability to rotate
    /// </summary>
    /// <param name="_state">The state to toggle to</param>
    public void ToggleRotation(bool _state)
    {
        CanRotate = _state;
    }

    void EnableRotation()
    {
        ToggleRotation(true);
    }

    void DisableRotation()
    {
        ToggleRotation(false);
    }

    void Update()
    {
        if (CanRotate) {
            Rotation();
        }
    }

    /// <summary>
    /// Rotates the player camera 
    /// </summary>
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
