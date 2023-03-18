using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTransition : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float smoothTime = .1f;

    Vector3 offset;
    Vector3 movVel;
    Vector3 rotVel;

    void Start()
    {
        offset = followTarget.position - transform.position;
    }

    void FixedUpdate()
    {
        Vector3 _targetPos = followTarget.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref movVel, smoothTime);
        //transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, followTarget.rotation.eulerAngles, ref rotVel, smoothTime));
    }
    void Update()
    {
        transform.rotation = followTarget.rotation;
    }
}
