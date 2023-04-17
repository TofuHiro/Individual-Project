using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParent : MonoBehaviour
{
    [Tooltip("The transform to follow")]
    [SerializeField] Transform followTarget;
    [Tooltip("The time taken to reach the new destination")]
    [SerializeField] float smoothTime = .1f;

    Vector3 vel;

    void FixedUpdate()
    {
        Vector3 _targetPos = followTarget.position;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref vel, smoothTime);
    }
    void Update()
    {
        transform.rotation = followTarget.rotation;
    }
}
