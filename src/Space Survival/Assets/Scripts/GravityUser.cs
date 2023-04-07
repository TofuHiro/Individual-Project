using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityUser : MonoBehaviour
{
    public delegate void GravityChange(bool _state);
    public event GravityChange OnChange;

    List<Collider> enteredColliders;
    Rigidbody rigidBody;

    bool state, prevState;

    void Awake()
    {
        enteredColliders = new List<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (state != prevState) {
            OnChange?.Invoke(state);
            prevState = state;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gravity") || other.CompareTag("OxygenGravity")) {
            rigidBody.useGravity = true;
            state = true;
            enteredColliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Gravity") || other.CompareTag("OxygenGravity")) {
            enteredColliders.Remove(other);
        }

        if (enteredColliders.Count == 0) {
            rigidBody.useGravity = false;
            state = false;
        }
    }
}
