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

    /// <summary>
    /// Rotate this enemy towards a given rotation
    /// </summary>
    /// <param name="_rot">The rotation to rotate towards</param>
    public void Rotate(Quaternion _rot)
    {
        head.rotation = Quaternion.RotateTowards(head.rotation, Quaternion.Euler(_rot.eulerAngles.x, _rot.eulerAngles.y, 0f), 50f * rotateSpeed * Time.deltaTime);
        orientation.rotation = Quaternion.RotateTowards(orientation.rotation, Quaternion.Euler(0f, _rot.eulerAngles.y, 0f), 50f * rotateSpeed * Time.deltaTime);
    }
}   
