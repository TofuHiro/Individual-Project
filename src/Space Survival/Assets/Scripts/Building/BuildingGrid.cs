using System.Collections.Generic;
using UnityEngine;

public class BuildingGrid
{
    class StructureSystem
    {
        readonly Vector3Int[] traverseDir =
        {
        new Vector3Int(0, -1, 0),   //Down
        new Vector3Int(0, 0, 1),    //Front
        new Vector3Int(1, 0, 0),    //Right
        new Vector3Int(0, 0, -1),   //Back
        new Vector3Int(-1, 0, 0),   //Left
        new Vector3Int(0, 1, 0),    //Up
        };

        GridStructure[,,] grid;
        Vector3Int gridDimension;
        int gridSize;

        public StructureSystem(int _gridSize)
        {
            gridSize = _gridSize;
        }

        public void CreateGrid(Vector3Int _size)
        {
            grid = new GridStructure[_size.x, _size.y, _size.z];
            gridDimension = _size;
        }

        public bool CheckPosConnected(Vector3 _worldSpacePos)
        {
            Vector3Int _gridPos = new Vector3Int((int)_worldSpacePos.x, (int)_worldSpacePos.y, (int)_worldSpacePos.z);
            bool _connected = false;
            for (int i = 0; i < 6; i++) {
                if (CheckPosForStructure(_gridPos + (traverseDir[i]))) {
                    _connected = true;
                }
            }
            return _connected;
        }

        bool CheckPosForStructure(Vector3Int _gridPos)
        {
            if (CheckPosInGrid(_gridPos))
                return false;
            else 
                return grid[_gridPos.x, _gridPos.y, _gridPos.z] != null;
        }

        bool CheckPosInGrid(Vector3Int _gridPos)
        {
            if (_gridPos.x < 0
                || _gridPos.x > gridDimension.x
                || _gridPos.y < 0
                || _gridPos.y > gridDimension.y
                || _gridPos.z < 0
                || _gridPos.z > gridDimension.z)
                return false;
            else
                return true;
        }

        public void AddFloor(Vector3Int _gridPos)
        {
            //Change grid size if need

            grid[_gridPos.x, _gridPos.y, _gridPos.z].Floor = true;
        }

        public bool CheckOverlapFloor(Vector3Int _gridPos)
        {
            if (!CheckPosInGrid(_gridPos))
                return false;
            else
                return grid[_gridPos.x, _gridPos.y, _gridPos.z].Floor == true;
        }

        public void AddEdge(Vector3Int _gridPos, Edge _edge)
        {
            //Change grid size if need

            grid[_gridPos.x, _gridPos.y, _gridPos.z].GetEdge(_edge) = true;
        }

        public bool CheckOverlapEdge(Vector3Int _gridPos, Edge _edge)
        {
            if (!CheckPosInGrid(_gridPos))
                return false;
            else
                return grid[_gridPos.x, _gridPos.y, _gridPos.z].GetEdge(_edge) == true;
        }
    }

    //Each point on the grid
    class GridStructure
    {
        //Each point will include these structures that may not overlap
        public bool Floor, FrontWall, RightWall, BackWall, LeftWall;
        public bool Sealed;

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

    public enum Edge
    {
        Front,
        Right,
        Back,
        Left
    }

    List<StructureSystem> systems;
    int gridSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_gridSize">The size of each unit in the building grid</param>
    public BuildingGrid(int _gridSize)
    {
        systems = new List<StructureSystem>();
        gridSize = _gridSize;
    }

    StructureSystem GetConnectedSystem(Vector3 _worldSpacePos)
    {
        int _connections = 0;
        List<StructureSystem> _connectedSystem = new List<StructureSystem>();
        foreach (StructureSystem _system in systems) {
            if (_system.CheckPosConnected(_worldSpacePos)) {
                _connectedSystem.Add(_system);
                _connections++;
            }
        }

        if (_connections == 1) {
            return _connectedSystem[0];
        }
        /*else if (_connections > 1) {
            return ConnectSystems(_connectedSystem);
        }*/
        else {
            return null;
        }
    }

    StructureSystem CreateNewSystem()
    {
        return new StructureSystem(gridSize);
    }

    /*StructureSystem ConnectSystems(List<StructureSystem> _systems)
    {

    }*/

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
                return AddEdge(_buildable.GetTargetPos(), RotationToEdge(_buildable.transform.rotation));

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

    public void RemoveStructure(Buildable _buildable)
    {
        switch (_buildable.GetBuildableType()) {
            case BuildableType.Floor:
                RemoveFloor(_buildable.transform.position);
                break;
            case BuildableType.Wall:
                RemoveEdge(_buildable.transform.position, RotationToEdge(_buildable.transform.rotation));
                break;
            default:
                break;
        }
    }

    bool AddFloor(Vector3 _worldSpacePos)
    {
        if (CheckOverlapFloor(_worldSpacePos)) 
            return false;

        StructureSystem _system = GetConnectedSystem(_worldSpacePos);
        if (_system == null) {
            _system = CreateNewSystem();
        }

        Vector3Int _gridPos = new Vector3Int((int)_worldSpacePos.x, (int)_worldSpacePos.y, (int)_worldSpacePos.z);
        _system.AddFloor(_gridPos);
        return true;
    }

    void RemoveFloor(Vector3 _worldSpacePos)
    {

    }

    bool CheckOverlapFloor(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = new Vector3Int((int)_worldSpacePos.x, (int)_worldSpacePos.y, (int)_worldSpacePos.z);

        foreach (StructureSystem _system in systems) {
            if (_system.CheckOverlapFloor(_gridPos)) {
                return true;
            }
        }
        return false;
    }

    bool AddEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        if (CheckOverlapEdge(_worldSpacePos, _edge))
            return false;

        StructureSystem _system = GetConnectedSystem(_worldSpacePos);
        if (_system == null) {
            _system = CreateNewSystem();
        }

        Vector3Int _gridPos = new Vector3Int((int)_worldSpacePos.x, (int)_worldSpacePos.y, (int)_worldSpacePos.z);
        _system.AddEdge(_gridPos, _edge);
        return true;
    }

    void RemoveEdge(Vector3 _worldSpacePos, Edge _edge)
    {

    }

    bool CheckOverlapEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        Vector3Int _gridPos = new Vector3Int((int)_worldSpacePos.x, (int)_worldSpacePos.y, (int)_worldSpacePos.z);

        foreach (StructureSystem _system in systems) {
            if (_system.CheckOverlapEdge(_gridPos, _edge)) {
                return true;
            }
        }
        return false;
    }

    Edge RotationToEdge(Quaternion _rot)
    {
        //Each unit is 90 deg. Assume 0 deg is foward, rotating clockwise, 90 - right, 180 - back, 270 - left
        int _unit = ((int)_rot.eulerAngles.y / 90);

        //Front
        if (_unit == 0) {
            return Edge.Front;
        }
        //Right
        else if (_unit == 1) {
            return Edge.Right;
        }
        //Back
        else if (_unit == 2) {
            return Edge.Back;
        }
        //Left
        else //if (_unit == 3) {
            return Edge.Left;
    }
}
