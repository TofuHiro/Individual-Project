using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    public bool CanPlaceWithoutSnaps { get { return canPlaceWithoutSnaps; } private set { canPlaceWithoutSnaps = value; } }
    public bool UseSnapPoints { get { return useSnapPoints; } private set { useSnapPoints = value; } }

    [SerializeField] BuildableType type;
    [SerializeField] bool canPlaceWithoutSnaps;
    [SerializeField] bool useSnapPoints;
    [SerializeField] GameObject snapPointsParent;
    [SerializeField] Vector3 gridSize;
    [SerializeField] Vector3 gridOffset;
  
    Collider col;
    Vector3 velocity;
    Vector3 targetPos;
    float smoothTime;

    void Start()
    {
        smoothTime = BuildingManager.Instance.GetBuildingSmoothTime();
    }

    public BuildableType GetBuildableType()
    {
        return type;
    }

    public Vector3 GetTargetPos()
    {
        return targetPos;
    }

    public void SetPosition(Vector3 _pos, bool snapToGrid)
    {
        if (snapToGrid) {
            _pos.x = Mathf.RoundToInt(_pos.x / gridSize.x) * gridSize.x;
            _pos.y = Mathf.RoundToInt(_pos.y / gridSize.y) * gridSize.y;
            _pos.z = Mathf.RoundToInt(_pos.z / gridSize.z) * gridSize.z;
            _pos += gridOffset;
        }
        targetPos = _pos;
        transform.position = Vector3.SmoothDamp(transform.position, _pos, ref velocity, smoothTime);
    }

    public void SetRotation(Quaternion _rot)
    {
        transform.rotation = _rot;
    }

    public void StartBluePrint()
    {
        if (col == null)
            col = gameObject.GetComponentInChildren<Collider>();
        col.isTrigger = true;
        snapPointsParent.SetActive(false);
    }

    public void Build()
    {
        if (col == null)
            col = gameObject.GetComponentInChildren<Collider>();
        col.isTrigger = false;
        snapPointsParent.SetActive(true);
    }
}
