using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenDistributer : MonoBehaviour
{
    [SerializeField] SphereCollider oxygenTrigger;

    void Start()
    {
        EnableOxygen();
    }

    void EnableOxygen()
    {
        oxygenTrigger.enabled = true;
    }

    void DisableOxygen()
    {
        oxygenTrigger.enabled = false;
    }
}
