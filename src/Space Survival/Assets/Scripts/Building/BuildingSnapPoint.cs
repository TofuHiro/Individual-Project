using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSnapPoint : MonoBehaviour
{
    [System.Serializable]
    public class BuildableSnapKVP
    {
        public bool IsOccupied { get; set; } 
        public BuildableType TargetBuildable;
        public Transform SnapPosition;
    }

    [SerializeField] BuildableSnapKVP[] buildableSnapPoints;

    public bool CheckForType(BuildableType _type)
    {
        for (int i = 0; i < buildableSnapPoints.Length; i++) {
            if (buildableSnapPoints[i].TargetBuildable == _type)
                return true;
        }
        return false;
    }

    public bool GetPointOccupied(BuildableType _type)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                return _point.IsOccupied;
            }
        }
        return false;
    }

    public void SetPointOccupied(BuildableType _type, bool _state)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                _point.IsOccupied = _state;
            }
        }
    }

    public Vector3 GetSnapPosition(BuildableType _type)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                return _point.SnapPosition.position;
            }
        }
        return Vector3.zero;
    }

    public Quaternion GetSnapRotation(BuildableType _type)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                return _point.SnapPosition.rotation;
            }
        }
        return Quaternion.Euler(Vector3.zero);
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < buildableSnapPoints.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(buildableSnapPoints[i].SnapPosition.position, buildableSnapPoints[i].SnapPosition.forward);
        }
    }
}
