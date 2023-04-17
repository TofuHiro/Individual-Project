using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothGrow : MonoBehaviour
{
    [Tooltip("The size to grow to")]
    [SerializeField] Vector3 growToSize;
    [Tooltip("The size to grow from")]
    [SerializeField] Vector3 growFromSize;
    [Tooltip("Whether to start to grow when this object's start method is called")]
    [SerializeField] bool growOnStart;
    [Tooltip("The time taken to grow")]
    [SerializeField] float growTime;

    bool isGrowing;

    void Start()
    {
        if (growOnStart) {
            Grow();
        }
    }

    public void Grow()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        transform.localScale = growFromSize;
        isGrowing = true;
    }

    void Update()
    {
        if (!isGrowing)
            return;

        transform.localScale = Vector3.Slerp(transform.localScale, growToSize, growTime);

        if (transform.localScale == growToSize)
            isGrowing = false;
    }
}
