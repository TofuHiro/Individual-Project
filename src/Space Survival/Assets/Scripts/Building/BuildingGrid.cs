using System.Collections.Generic;
using UnityEngine;

public class BuildingGrid
{
    List<StructureSystem> systems;
    int gridUnit;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_gridUnit">The size of each unit in the building grid</param>
    public BuildingGrid(int _gridUnit)
    {
        systems = new List<StructureSystem>();
        gridUnit = _gridUnit;
    }

    /// <summary>
    /// Creates a new building system 
    /// </summary>
    /// <param name="_worldSpacePos">The origin position of the new system</param>
    /// <returns>Returns the new system created</returns>
    StructureSystem CreateNewSystem(Vector3 _worldSpacePos)
    {
        StructureSystem _newSystem = new StructureSystem(_worldSpacePos, Vector3Int.one,gridUnit);
        systems.Add(_newSystem);

        return _newSystem;
    }

    /// <summary>
    /// Returns the system if there is an exisiting structure at the given position
    /// </summary>
    /// <param name="_worldSpacePos">The world space position to check</param>
    /// <returns>Returns the system at the position</returns>
    StructureSystem GetSystem(Vector3 _worldSpacePos)
    {
        foreach (StructureSystem _system in systems) {
            if (_system.CheckPosForStructure(_worldSpacePos)) {
                return _system;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns a system for the given position to join
    /// </summary>
    /// <param name="_worldSpacePos">The position to find a system for</param>
    /// <returns>Returns a system that links to the position</returns>
    StructureSystem JoinSystem(Vector3 _worldSpacePos)
    {
        if (GetSystem(_worldSpacePos) != null) {
            return GetSystem(_worldSpacePos);
        }

        List<StructureSystem> _systems = GetConnectedSystems(_worldSpacePos);
        //No connected systems
        if (_systems.Count == 0) {
            return CreateNewSystem(_worldSpacePos);
        }
        //1 or more systems
        else {
            //Connect all systems to one
            for (int i = 1; i < _systems.Count; i++) {
                _systems[0] = ConnectSystems(_systems[0], _systems[i]);
            }
            return _systems[0];
        }
    }

    /// <summary>
    /// Gets all systems that are connected to the given position
    /// </summary>
    /// <param name="_worldSpacePos">The position to check for systems</param>
    /// <returns>A list of systems that are connected to the position</returns>
    List<StructureSystem> GetConnectedSystems(Vector3 _worldSpacePos)
    {
        List<StructureSystem> _connectedSystems = new List<StructureSystem>();
        foreach (StructureSystem _system in systems) {
            if (_system.CheckPosConnected(_worldSpacePos)) {
                _connectedSystems.Add(_system);
            }
        }

        return _connectedSystems;
    }

    /// <summary>
    /// Connects 2 systems together and returns the connected system
    /// </summary>
    /// <param name="_first"></param>
    /// <param name="_second"></param>
    /// <returns>The connected systems</returns>
    StructureSystem ConnectSystems(StructureSystem _first, StructureSystem _second)
    {
        //Check size between one end of one system to highest end of other system
        int _xMax = Mathf.Max((int)(_second.OriginWorldPos.x + (_second.GridSize.x * gridUnit) - _first.OriginWorldPos.x) / gridUnit, (int)(_first.OriginWorldPos.x + (_first.GridSize.x * gridUnit) - _second.OriginWorldPos.x) / gridUnit);
        //Check if sizes of grids are bigger
        _xMax = Mathf.Max(_xMax, _first.GridSize.x);
        _xMax = Mathf.Max(_xMax, _second.GridSize.x);

        int _yMax = Mathf.Max((int)(_second.OriginWorldPos.y + (_second.GridSize.y * gridUnit) - _first.OriginWorldPos.y) / gridUnit, (int)(_first.OriginWorldPos.y + (_first.GridSize.y * gridUnit) - _second.OriginWorldPos.y) / gridUnit);
        _yMax = Mathf.Max(_yMax, _first.GridSize.y);
        _yMax = Mathf.Max(_yMax, _second.GridSize.y);
        
        int _zMax = Mathf.Max((int)(_second.OriginWorldPos.z + (_second.GridSize.z * gridUnit) - _first.OriginWorldPos.z) / gridUnit, (int)(_first.OriginWorldPos.z + (_first.GridSize.z * gridUnit) - _second.OriginWorldPos.z) / gridUnit);
        _zMax = Mathf.Max(_zMax, _first.GridSize.z);
        _zMax = Mathf.Max(_zMax, _second.GridSize.z);

        Vector3 _newOrigin = Vector3.Min(_first.OriginWorldPos, _second.OriginWorldPos);
        StructureSystem _newSystem = new StructureSystem(_newOrigin, new Vector3Int(_xMax, _yMax, _zMax), gridUnit);

        Vector3Int _firstOffset = Vector3Int.RoundToInt(_first.OriginWorldPos - _newOrigin) / gridUnit;
        Vector3Int _secondOffset = Vector3Int.RoundToInt(_second.OriginWorldPos - _newOrigin) / gridUnit;

        _newSystem.CombineGrid(_first.GetGrid(), _first.GridSize, _firstOffset);
        _newSystem.CombineGrid(_second.GetGrid(), _second.GridSize, _secondOffset);

        systems.Remove(_first);
        systems.Remove(_second);
        systems.Add(_newSystem);

        return _newSystem;
    }

    /// <summary>
    /// Splits a system to many others given a point to split from
    /// </summary>
    /// <param name="_system">The system to split</param>
    /// <param name="_splitPoint">The point to split the system from</param>
    void SplitSystem(StructureSystem _system, Vector3 _splitPoint)
    {
        List<StructureSystem> _newSystems = _system.SplitSystem(_splitPoint);
        foreach (StructureSystem _newSystem in _newSystems) {
            systems.Add(_newSystem);
        }
        systems.Remove(_system);
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

    /// <summary>
    /// Removes a given structure from the grid
    /// </summary>
    /// <param name="_buildable"></param>
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

    /// <summary>
    /// Attempts to add a floor to a given position
    /// </summary>
    /// <param name="_worldSpacePos">The position to add the floor to</param>
    /// <returns>Returns false if overlapping another floor, otherwise true if successful</returns>
    bool AddFloor(Vector3 _worldSpacePos)
    {
        if (CheckOverlapFloor(_worldSpacePos)) 
            return false;

        JoinSystem(_worldSpacePos).AddFloor(_worldSpacePos);
        return true;
    }

    /// <summary>
    /// Removes a floor at a given position and splits the system if needed
    /// </summary>
    /// <param name="_worldSpacePos">The position to remove the floor at</param>
    void RemoveFloor(Vector3 _worldSpacePos)
    {
        StructureSystem _system = GetSystem(_worldSpacePos);
        int _splitPoints = _system.RemoveFloor(_worldSpacePos);
        if (_splitPoints == 0) {
            systems.Remove(_system);
        }
        else if (_splitPoints > 1) {
            SplitSystem(_system, _worldSpacePos);
        }
    }

    /// <summary>
    /// Checks a position if a floor exists
    /// </summary>
    /// <param name="_worldSpacePos">The position to check at</param>
    /// <returns>Returns true if overlapping another floor</returns>
    bool CheckOverlapFloor(Vector3 _worldSpacePos)
    {
        foreach (StructureSystem _system in systems) {
            if (_system.CheckOverlapFloor(_worldSpacePos)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to add an edge to a given position
    /// </summary>
    /// <param name="_worldSpacePos">The position to add the edge to</param>
    /// <param name="_edge">The edge type to add</param>
    /// <returns>Returns false if overlapping another edge, otherwise true if successful</returns>
    bool AddEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        if (CheckOverlapEdge(_worldSpacePos, _edge))
            return false;

        JoinSystem(_worldSpacePos).AddEdge(_worldSpacePos, _edge);
        return true;
    }

    /// <summary>
    /// Removes an edge at the given position and splits the system if needed
    /// </summary>
    /// <param name="_worldSpacePos">The position to remove the edge at</param>
    /// <param name="_edge">The type of edge to remove</param>
    void RemoveEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        StructureSystem _system = GetSystem(_worldSpacePos);
        int _splitPoints = _system.RemoveEdge(_worldSpacePos, _edge);
        if (_splitPoints == 0) {
            systems.Remove(_system);
        }
        else if (_splitPoints > 1) {
            SplitSystem(_system, _worldSpacePos);
        }
    }

    /// <summary>
    /// Checks a position if an edge exists
    /// </summary>
    /// <param name="_worldSpacePos">The position to check at</param>
    /// <param name="_edge">The type of edge to check for</param>
    /// <returns>Returns true if overlapping another edge</returns>
    bool CheckOverlapEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        foreach (StructureSystem _system in systems) {
            if (_system.CheckOverlapEdge(_worldSpacePos, _edge)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns an edge type depending on a rotation
    /// </summary>
    /// <param name="_rot">The rotation to convert from</param>
    /// <returns>Edge enum of the type of edge</returns>
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
