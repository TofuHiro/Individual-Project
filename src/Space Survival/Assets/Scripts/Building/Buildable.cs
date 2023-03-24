using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    public ItemScriptable ItemInfo { get { return itemInfo; } private set { itemInfo = value; } }

    [Tooltip("The item scriptable object for this buildable")]
    [SerializeField] ItemScriptable itemInfo;
    [Tooltip("The physical model game object. Make sure that the desired layer is set on this object")]
    [SerializeField] GameObject modelObject;

    [Header("Building Settings")]
    [Tooltip("The type of building this buildable is")]
    [SerializeField] BuildableType type;
    [Tooltip("The grid size this building will snap to when being placed")]
    [SerializeField] Vector3 gridSize;

    Vector3 velocity;
    Vector3 targetPos;
    float smoothTime;

    void Start()
    {
        smoothTime = BuildingManager.Instance.GetBuildingSmoothTime();
    }

    /// <summary>
    /// Returns the buildable type of this buildable
    /// </summary>
    /// <returns>The buildable type of this buildable</returns>
    public BuildableType GetBuildableType()
    {
        return type;
    }

    /// <summary>
    /// The position that this buildable is set to move to ignoreing the transitioning position
    /// </summary>
    /// <returns>The vector3 position where this building is moving to</returns>
    public Vector3 GetTargetPos()
    {
        return targetPos;
    }

    /// <summary>
    /// Returns the model game objects of the buildable
    /// </summary>
    /// <returns>Game object of the buildable model</returns>
    public GameObject GetObject()
    {
        return modelObject;
    }

    /// <summary>
    /// Set the position for this buildable to move to
    /// </summary>
    /// <param name="_pos">The vector3 position to move to</param>
    public void SetPosition(Vector3 _pos)
    {
        _pos.x = Mathf.RoundToInt(_pos.x / gridSize.x) * gridSize.x;
        _pos.y = Mathf.RoundToInt(_pos.y / gridSize.y) * gridSize.y;
        _pos.z = Mathf.RoundToInt(_pos.z / gridSize.z) * gridSize.z;
        
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
        foreach (Collider _col in GetComponentsInChildren<Collider>()) {
            _col.enabled = false;
        }
    }

    /// <summary>
    /// Finalizes this buildable
    /// </summary>
    public void Build()
    {
        foreach (Collider _col in GetComponentsInChildren<Collider>()) {
            _col.enabled = true;
        }
    }
}
