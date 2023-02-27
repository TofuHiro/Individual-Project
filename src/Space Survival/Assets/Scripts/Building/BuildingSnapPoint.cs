using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSnapPoint : MonoBehaviour
{
    public bool IsOccupied { get; set; }

    [System.Serializable]
    class BuildableSnapKVP
    {
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

    public Vector3 GetSnapPosition(BuildableType _type)
    {
        for (int i = 0; i < buildableSnapPoints.Length; i++) {
            if (buildableSnapPoints[i].TargetBuildable == _type)
                return buildableSnapPoints[i].SnapPosition.position;
        }
        return Vector3.zero;
    }

    public Quaternion GetSnapRotation(BuildableType _type)
    {
        for (int i = 0; i < buildableSnapPoints.Length; i++) {
            if (buildableSnapPoints[i].TargetBuildable == _type)
                return buildableSnapPoints[i].SnapPosition.rotation;
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
