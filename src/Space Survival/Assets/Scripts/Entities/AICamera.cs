using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICamera : MonoBehaviour
{
    [Tooltip("The transform indicating the head of this enemy")]
    [SerializeField] Transform head;
    [Tooltip("The transform storing the orientation of this enemy")]
    [SerializeField] Transform orientation;
    [Tooltip("The speed of which this enemy rotates at")]
    [SerializeField] float rotateSpeed = 10f;

    bool isFloating;
    Quaternion rot;

    /// <summary>
    /// Returns the orientation transform of this enemy
    /// </summary>
    /// <returns></returns>
    public Transform GetOrientation()
    {
        return orientation;
    }

    /// <summary>
    /// Sets the rotation to rotate to
    /// </summary>
    /// <param name="_rot">The rotation to rotate towards</param>
    public void SetRotation(Quaternion _rot, bool _isFloating)
    {
        rot = _rot;
        isFloating = _isFloating;
    }

    public void LookAt(Quaternion _rot, bool _isFloating)
    {
        rot = _rot;

        if (_isFloating) {
            orientation.rotation = Quaternion.Euler(_rot.eulerAngles.x, _rot.eulerAngles.y, 0f);
        }
        else {
            head.rotation = Quaternion.Euler(_rot.eulerAngles.x, _rot.eulerAngles.y, 0f);
            orientation.rotation = Quaternion.Euler(0f, _rot.eulerAngles.y, 0f);
        }
    }

    void Update()
    {
        if (isFloating) {
            orientation.rotation = Quaternion.RotateTowards(orientation.rotation, Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0f), 50f * rotateSpeed * Time.deltaTime);
        }
        else {
            head.rotation = Quaternion.RotateTowards(head.rotation, Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0f), 50f * rotateSpeed * Time.deltaTime);
            orientation.rotation = Quaternion.RotateTowards(orientation.rotation, Quaternion.Euler(0f, rot.eulerAngles.y, 0f), 50f * rotateSpeed * Time.deltaTime);
        }
    }
}   
