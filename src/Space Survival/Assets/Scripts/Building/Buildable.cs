using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    public bool CanPlaceWithoutSnaps { get { return canPlaceWithoutSnaps; } private set { canPlaceWithoutSnaps = value; } }
    public bool UseSnapPoints { get { return useSnapPoints; } private set { useSnapPoints = value; } }

    [Tooltip("The parent game object holding all the snap points")]
    [SerializeField] GameObject snapPointsParent;
    [Tooltip("The type of construction this building is")]
    [SerializeField] BuildableType type;

    [Header("Positioning Settings")]
    [Tooltip("If this buildable is able to be placed without requiring a snapping point from another building")]
    [SerializeField] bool canPlaceWithoutSnaps;
    [Tooltip("If this buildable uses other building's snap points to position itself")]
    [SerializeField] bool useSnapPoints;
    [Tooltip("The grid size this building will snap to when being placed")]
    [SerializeField] Vector3 gridSize;
    [Tooltip("The offset that is applied when positioning this building")]
    [SerializeField] Vector3 gridOffset;

    Collider col;
    Vector3 velocity;
    Vector3 targetPos;
    float smoothTime;

    void Start()
    {
        smoothTime = BuildingManager.Instance.GetBuildingSmoothTime();
    }

    /// <summary>
    /// Returns the type of construction this building is
    /// </summary>
    /// <returns>BuildableType enum of this buildable</returns>
    public BuildableType GetBuildableType()
    {
        return type;
    }

    /// <summary>
    /// The position that this buildable is set to move towards and be placed at
    /// </summary>
    /// <returns>The vector3 position where this building is moving towards</returns>
    public Vector3 GetTargetPos()
    {
        return targetPos;
    }

    /// <summary>
    /// Set the position for this buildable to move to
    /// </summary>
    /// <param name="_pos">The vector3 position to move to</param>
    /// <param name="snapToGrid">Whether to snap to the given grid size of this buildable or not</param>
    public void SetPosition(Vector3 _pos, bool snapToGrid)
    {
        if (snapToGrid) {
            _pos.x = Mathf.RoundToInt(_pos.x / gridSize.x) * gridSize.x;
            _pos.y = Mathf.RoundToInt(_pos.y / gridSize.y) * gridSize.y;
            _pos.z = Mathf.RoundToInt(_pos.z / gridSize.z) * gridSize.z;
            _pos += gridOffset;
        }
        targetPos = _pos;
        
        //movement interpolation
        transform.position = Vector3.SmoothDamp(transform.position, _pos, ref velocity, smoothTime);
    }

    /// <summary>
    /// Sets the rotation of this buildable
    /// </summary>
    /// <param name="_rot">The rotation to set to</param>
    public void SetRotation(Quaternion _rot)
    {
        transform.rotation = _rot;
    }

    /// <summary>
    /// Turn this buildable into a blueprint to be placed
    /// </summary>
    public void StartBlueprint()
    {
        if (col == null)
            col = gameObject.GetComponentInChildren<Collider>();
        col.isTrigger = true;
        snapPointsParent.SetActive(false);
    }

    /// <summary>
    /// Finalizes this buildable
    /// </summary>
    public void Build()
    {
        if (col == null)
            col = gameObject.GetComponentInChildren<Collider>();
        col.isTrigger = false;
        snapPointsParent.SetActive(true);
    }
}
