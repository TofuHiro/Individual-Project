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

    [Tooltip("All the snap positions and their types of this snap point")]
    [SerializeField] BuildableSnapKVP[] buildableSnapPoints;

    /// <summary>
    /// Checks for the given buildable type if there is an existing snap position for this snap point
    /// </summary>
    /// <param name="_type">The buildable type to check for</param>
    /// <returns>Returns true if there is an existing snap position</returns>
    public bool CheckForType(BuildableType _type)
    {
        for (int i = 0; i < buildableSnapPoints.Length; i++) {
            if (buildableSnapPoints[i].TargetBuildable == _type)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the occupancy of a snap position
    /// </summary>
    /// <param name="_type">The buildable type of the snap position to check for</param>
    /// <returns>The occupancy of the snap position of the given type</returns>
    public bool GetPointOccupied(BuildableType _type)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                return _point.IsOccupied;
            }
        }
        return false;
    }

    /// <summary>
    /// Sets the occupancy of a snap position
    /// </summary>
    /// <param name="_type">The buildable type of the snap position</param>
    /// <param name="_state">The state to set the point to</param>
    public void SetPointOccupied(BuildableType _type, bool _state)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                _point.IsOccupied = _state;
            }
        }
    }

    /// <summary>
    /// Returns the position of the given snap position of the given buildable type
    /// </summary>
    /// <param name="_type">The buildable type of the snap position</param>
    /// <returns>The vector3 position of the snap position of the given type</returns>
    public Vector3 GetSnapPosition(BuildableType _type)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                return _point.SnapPosition.position;
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Returns the rotation of the given snap position of the given buildable type
    /// </summary>
    /// <param name="_type">The buildable type of the snap position</param>
    /// <returns>The quaternion rotation of the snap position of the given type</returns>
    public Quaternion GetSnapRotation(BuildableType _type)
    {
        foreach (BuildableSnapKVP _point in buildableSnapPoints) {
            if (_point.TargetBuildable == _type) {
                return _point.SnapPosition.rotation;
            }
        }
        return Quaternion.Euler(Vector3.zero);
    }

    //Displays lines pointing foward for each snap position to manage rotations easier for placement
    void OnDrawGizmos()
    {
        for (int i = 0; i < buildableSnapPoints.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(buildableSnapPoints[i].SnapPosition.position, buildableSnapPoints[i].SnapPosition.forward);
        }
    }
}
