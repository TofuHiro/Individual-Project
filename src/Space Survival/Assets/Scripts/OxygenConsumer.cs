using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenConsumer : MonoBehaviour
{
    public bool IsSuffocating { get; private set; }

    [Tooltip("The rate of oxygen recovery")]
    [SerializeField] float oxygenRecoverRate = 10f;
    [Tooltip("The starting maximum oxygen breathing time")]
    [SerializeField] float maxOxygenTime = 60f;

    BuildingGrid grid;
    StructureSystem currentSystem;

    int gridUnit;
    float oxygen;
    bool inOxygen, inOxygenCollider;

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

    public void AddOxygen(float _value)
    {
        oxygen += _value;
    }

    public void SetMax()
    {
        oxygen = maxOxygenTime;
    }

    void Start()
    {
        gridUnit = BuildingGrid.Instance.GetGridUnit();
        grid = BuildingGrid.Instance;
        oxygen = maxOxygenTime;
    }

    Vector3 _unitPos;
    void Update()
    {
        //If in trigger, default to true
        if (inOxygenCollider) {
            inOxygen = true;
        }
        //If not in trigger, check if in a sealed system
        else {
            _unitPos = transform.position;
            _unitPos.x = Mathf.FloorToInt((_unitPos.x + 2) / gridUnit) * gridUnit;
            _unitPos.y = Mathf.FloorToInt(_unitPos.y / gridUnit) * gridUnit;
            _unitPos.z = Mathf.FloorToInt((_unitPos.z + 2) / gridUnit) * gridUnit;
            currentSystem = grid.GetSystem(_unitPos);
            //If in system check if its sealed.
            if (currentSystem != null) {
                inOxygen = currentSystem.CheckPosIsSealed(_unitPos);
            }
            else
                inOxygen = false;
        }

        if (!inOxygen) 
            oxygen -= Time.deltaTime;
        else 
            oxygen += Time.deltaTime * oxygenRecoverRate;

        oxygen = Mathf.Clamp(oxygen, 0f, maxOxygenTime);
        IsSuffocating = (oxygen <= 0f);
    }

    //Determine whether player is in oxygen or not
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Oxygen")) {
            inOxygenCollider = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Oxygen")) {
            inOxygenCollider = false;
        }
    }
}
