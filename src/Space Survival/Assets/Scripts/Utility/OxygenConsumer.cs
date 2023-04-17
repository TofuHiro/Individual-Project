using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenConsumer : MonoBehaviour
{
    public bool IsSuffocating { get; private set; }
    public bool InOxygen { get; private set; }

    [Tooltip("The rate of oxygen recovery")]
    [SerializeField] float oxygenRecoverRate = 10f;
    [Tooltip("The starting maximum oxygen breathing time")]
    [SerializeField] float maxOxygenTime = 60f;

    List<Collider> enteredColliders;
    BuildingGrid grid;
    StructureSystem currentSystem;

    Vector3 unitPos;
    int gridUnit;
    float oxygen;
    bool inOxygenCollider;

    public float GetOxygen()
    {
        return oxygen;
    }

    public float GetMaxOxygen()
    {
        return maxOxygenTime;
    }

    public void SetMaxOxygenTime(float _value)
    {
        maxOxygenTime = _value;
    }

    public void SetOxygen(float _value)
    {
        oxygen = _value;
    }

    public void SetMax()
    {
        oxygen = maxOxygenTime;
    }

    void Awake()
    {
        enteredColliders = new List<Collider>();
    }

    void Start()
    {
        grid = BuildingGrid.Instance;
        gridUnit = grid.GetGridUnit();
        
        oxygen = maxOxygenTime;
    }

    void Update()
    {
        //If in trigger, default to true
        if (inOxygenCollider) {
            InOxygen = true;
        }
        //If not in trigger, check if in a sealed system
        else {
            unitPos = transform.position;
            unitPos.x = Mathf.FloorToInt((unitPos.x + 2) / gridUnit) * gridUnit;
            unitPos.y = Mathf.FloorToInt(unitPos.y / gridUnit) * gridUnit;
            unitPos.z = Mathf.FloorToInt((unitPos.z + 2) / gridUnit) * gridUnit;
            currentSystem = grid.GetSystem(unitPos);
            //If in system check if its sealed.
            if (currentSystem != null) {
                InOxygen = currentSystem.CheckPosIsSealed(unitPos);
            }
            else
                InOxygen = false;
        }

        if (!InOxygen) 
            oxygen -= Time.deltaTime;
        else 
            oxygen += Time.deltaTime * oxygenRecoverRate;

        oxygen = Mathf.Clamp(oxygen, 0f, maxOxygenTime);
        IsSuffocating = (oxygen <= 0f);
    }

    //Determine whether player is in oxygen or not
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oxygen") || other.CompareTag("OxygenGravity")) {
            inOxygenCollider = true;
            enteredColliders.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oxygen") || other.CompareTag("OxygenGravity")) {
            RemoveCollider(other);
        }
    }

    public void RemoveCollider(Collider _collider)
    {
        enteredColliders.Remove(_collider);

        if (enteredColliders.Count == 0) {
            inOxygenCollider = false;
        }
    }
}
