using System.Collections.Generic;
using UnityEngine;

public class BuildingGrid
{
    public enum Edge
    {
        Front,
        Right,
        Back,
        Left
    }

    //Each point on the grid
    class GridStructure
    {
        public Vector3 Point;
        //Each point will include these structures that may not overlap
        public bool Floor, FrontWall, RightWall, BackWall, LeftWall;

        /// <summary>
        /// Get the reference of the corresponding edge bool
        /// </summary>
        /// <param name="_edge">The edge to get</param>
        /// <returns>The bool ref of the given edge</returns>
        public ref bool GetEdge(Edge _edge)
        {
            switch (_edge) {
                case Edge.Right:
                    return ref RightWall;
                case Edge.Back:
                    return ref BackWall;
                case Edge.Left:
                    return ref LeftWall;
                default:
                    //Edge.Front
                    return ref FrontWall;
            }
        }
    }
    
    Dictionary<Vector3, GridStructure> gridConstructions;
    int gridSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_gridSize">The size of each unit in the building grid</param>
    public BuildingGrid(int _gridSize)
    {
        gridConstructions = new Dictionary<Vector3, GridStructure>();
        gridSize = _gridSize;
    }

    /// <summary>
    /// Add a structure to the grid
    /// </summary>
    /// <param name="_buildable">The structure to add</param>
    /// <returns>Returns true if the structure is not overlapping another</returns>
    public bool AddStructure(Buildable _buildable)
    {
        switch (_buildable.GetBuildableType()) {
            case BuildableType.Floor:
                return AddFloor(_buildable.GetTargetPos());
            case BuildableType.Wall:
                return AddEdge(_buildable.GetTargetPos(), _buildable.transform.rotation);
            case BuildableType.Misc:
                return Physics.OverlapBox(
                    _buildable.GetObject().transform.position,  //Center of actual model object
                    _buildable.transform.localScale / 2.05f,    //Slightly smaller to avoid edge detection
                    _buildable.transform.rotation,
                    ~0,                                         //All layers
                    QueryTriggerInteraction.Ignore)             //Ignore trigger colliders
                    .Length == 0;                               //Check for overlapping collider, true if none
        default:
                return true;
        }
    }

    /// <summary>
    /// Removes a given structure from the building grid
    /// </summary>
    /// <param name="_buildable">The buildable to remove</param>
    public void RemoveStructure(Buildable _buildable)
    {
        switch (_buildable.GetBuildableType()) {
            case BuildableType.Floor:
                RemoveFloor(_buildable.transform.position);
                break;
            case BuildableType.Wall:
                RemoveEdge(_buildable.transform.position, _buildable.transform.rotation);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Add a floor to a grid point
    /// </summary>
    /// <param name="_pos">The world space position of the buildable</param>
    /// <returns>Returns true if the structure is not overlapping another</returns>
    bool AddFloor(Vector3 _pos)
    {
        //World space pos unit to normalized grid unit
        _pos /= gridSize;

        //If grid empty, initialize grid
        if (!gridConstructions.ContainsKey(_pos)) {
            gridConstructions.Add(_pos, new GridStructure());
        }

        //Check floor occupancy
        if (gridConstructions[_pos].Floor == true) {
            return false;
        }
        else {
            gridConstructions[_pos].Floor = true;
            return true;
        }

    }

    /// <summary>
    /// Removes a floor from the grid
    /// </summary>
    /// <param name="_pos">The position of the floor</param>
    void RemoveFloor(Vector3 _pos)
    {
        //World space pos unit to normalized grid unit
        _pos /= gridSize;

        gridConstructions[_pos].Floor = false;
    }

    /// <summary>
    /// Add an edge to a grid point
    /// </summary>
    /// <param name="_pos">The world space position of the buildable</param>
    /// <param name="_rot">The rotation of the buildable</param>
    /// <returns>Returns true if the structure is not overlapping another</returns>
    bool AddEdge(Vector3 _pos, Quaternion _rot)
    {
        //World space pos unit to normalized grid unit
        _pos /= gridSize;

        //If grid empty, initialize grid
        if (!gridConstructions.ContainsKey(_pos)) {
            gridConstructions.Add(_pos, new GridStructure());
        }

        //Each unit is 90 deg. Assume 0 deg is foward, rotating clockwise, 90 - right, 180 - back, 270 - left
        int _unit = ((int)_rot.eulerAngles.y / 90);

        //Front
        if (_unit == 0) {
            //Check edge occupancy
            if (gridConstructions[_pos].GetEdge(Edge.Front) == true) {
                return false;
            }
            //If not, assign as occupied
            else {
                gridConstructions[_pos].GetEdge(Edge.Front) = true;
                return true;
            }
        }
        //Right
        else if (_unit == 1) {
            if (gridConstructions[_pos].GetEdge(Edge.Right) == true) {
                return false;
            }
            else {
                gridConstructions[_pos].GetEdge(Edge.Right) = true;
                return true;
            }
        }
        //Back
        else if (_unit == 2) {
            if (gridConstructions[_pos].GetEdge(Edge.Back) == true) {
                return false;
            }
            else {
                gridConstructions[_pos].GetEdge(Edge.Back) = true;
                return true;
            }
        }
        //Left
        else if (_unit == 3) {
            if (gridConstructions[_pos].GetEdge(Edge.Left) == true) {
                return false;
            }
            else {
                gridConstructions[_pos].GetEdge(Edge.Left) = true;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Remove an edge on the grid
    /// </summary>
    /// <param name="_pos">The position of the edge</param>
    /// <param name="_rot">The rotation of the edge</param>
    void RemoveEdge(Vector3 _pos, Quaternion _rot)
    {
        //World space pos unit to normalized grid unit
        _pos /= gridSize;

        //Each unit is 90 deg. Assume 0 deg is foward, rotating clockwise, 90 - right, 180 - back, 270 - left
        int _unit = ((int)_rot.eulerAngles.y / 90);

        //Front
        if (_unit == 0) {
            gridConstructions[_pos].GetEdge(Edge.Front) = false;
        }
        //Right
        else if (_unit == 1) {
            gridConstructions[_pos].GetEdge(Edge.Right) = false;
        }
        //Back
        else if (_unit == 2) {
            gridConstructions[_pos].GetEdge(Edge.Back) = false;
        }
        //Left
        else if (_unit == 3) {
            gridConstructions[_pos].GetEdge(Edge.Left) = false;
        }
    }
}
