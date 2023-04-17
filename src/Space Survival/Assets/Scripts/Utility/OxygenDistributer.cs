using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenDistributer : MonoBehaviour
{
    [SerializeField] Collider oxygenTrigger;
    [SerializeField] bool enabledOnStart;
    [Tooltip("The buildable this distributer at attached to. Distributer will enable when the buildable is built")]
    [SerializeField] Buildable buildable;

    List<OxygenConsumer> consumers;

    void Start()
    {
        consumers = new List<OxygenConsumer>();

        if (enabledOnStart) {
            EnableOxygen();
        }
    }

    void OnEnable()
    {
        buildable.OnBuild += EnableOxygen;
    }

    void OnDisable()
    {
        buildable.OnBuild -= EnableOxygen;

        //Remove from list of conumser
        foreach (OxygenConsumer _consumer in consumers) {
            _consumer.RemoveCollider(oxygenTrigger);
        }
    }

    void OnDestroy()
    {
        buildable.OnBuild -= EnableOxygen;

        //Remove from list of conumser
        foreach (OxygenConsumer _consumer in consumers) {
            _consumer.RemoveCollider(oxygenTrigger);
        }
    }

    void EnableOxygen()
    {
        oxygenTrigger.gameObject.SetActive(true);
    }

    void DisableOxygen()
    {
        oxygenTrigger.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        OxygenConsumer _consumer = other.GetComponent<OxygenConsumer>();
        if (_consumer != null) {
            consumers.Add(_consumer);
        }
    }
}
