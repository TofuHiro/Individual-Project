using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureSystem
{
    readonly Vector3Int[] traverseDir =
    {
        new Vector3Int(0, 0, -1),   //Down
        new Vector3Int(0, 1, 0),    //Front
        new Vector3Int(1, 0, 0),    //Right
        new Vector3Int(0, -1, 0),   //Back
        new Vector3Int(-1, 0, 0),   //Left
        new Vector3Int(0, 0, 1),    //Up
    };

    public Vector3Int GridSize { get; private set; }
    public Vector3 OriginWorldPos { get; private set; }

    GridStructure[,,] grid;
    int gridUnit;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_originWorldPos">The starting world space position of the system</param>
    /// <param name="_gridSize">The 3D size of the grid</param>
    /// <param name="_gridUnit">The unit size of the building grid</param>
    public StructureSystem(Vector3 _originWorldPos, Vector3Int _gridSize, int _gridUnit)
    {
        CreateGrid(_gridSize, _originWorldPos);
        gridUnit = _gridUnit;
    }

    /// <summary>
    /// Returns the current grid of this system
    /// </summary>
    /// <returns>3D array of the grid</returns>
    public GridStructure[,,] GetGrid()
    {
        return grid;
    }

    /// <summary>
    /// Set a grid position to a new grid structure
    /// </summary>
    /// <param name="_gridPos">The grid index</param>
    /// <param name="_gridStructure">The grid structure to set</param>
    void SetGridPos(Vector3Int _gridPos, GridStructure _gridStructure)
    {
        grid[_gridPos.x, _gridPos.y, _gridPos.z] = _gridStructure;
    }

    /// <summary>
    /// Creates a new 3-dimensional grid of a given size
    /// </summary>
    /// <param name="_size">The 3D integer size of the grid</param>
    /// <param name="_originPos">The world position of the grid coords [0,0,0]</param>
    public void CreateGrid(Vector3Int _size, Vector3 _originPos)
    {
        grid = new GridStructure[_size.x, _size.y, _size.z];
        GridSize = _size;
        OriginWorldPos = _originPos;

        for (int x = 0; x < _size.x; x++) {
            for (int y = 0; y < _size.y; y++) {
                for (int z = 0; z < _size.z; z++) {
                    grid[x, y, z] = new GridStructure();
                }
            }
        }
    }

    /// <summary>
    /// Checks if a given position is connecting/touching this system
    /// </summary>
    /// <param name="_worldSpacePos">The position to check</param>
    /// <returns>Returns true if the position is connected to this system</returns>
    public bool CheckPosConnected(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);
        bool _connected = false;
        for (int i = 0; i < 6; i++) {
            if (CheckGridPosForStructure(_gridPos + (traverseDir[i]))) {
                _connected = true;
            }
        }
        return _connected;
    }

    /// <summary>
    /// Checks if any structure is built on at an index of the grid
    /// </summary>
    /// <param name="_worldSpacePos">The world space position to check</param>
    /// <returns>Returns true if any structure exists on the given position</returns>
    public bool CheckPosForStructure(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (!CheckPosInGrid(_gridPos))
            return false;
        else
            return !grid[_gridPos.x, _gridPos.y, _gridPos.z].IsEmpty;
    }

    bool CheckGridPosForStructure(Vector3Int _gridPos)
    {
        if (!CheckPosInGrid(_gridPos))
            return false;
        else
            return !grid[_gridPos.x, _gridPos.y, _gridPos.z].IsEmpty;
    }

    /// <summary>
    /// Checks if an index is within the current grid or not
    /// </summary>
    /// <param name="_gridPos">The grid index to check</param>
    /// <returns>Returns true if the index is within the grid</returns>
    bool CheckPosInGrid(Vector3 _gridPos)
    {
        if (_gridPos.x < 0
            || _gridPos.x > (GridSize.x - 1)
            || _gridPos.y < 0
            || _gridPos.y > (GridSize.y - 1)
            || _gridPos.z < 0
            || _gridPos.z > (GridSize.z - 1))
            return false;
        else
            return true;
    }

    /// <summary>
    /// Adds a floor to the grid at the given position
    /// </summary>
    /// <param name="_worldSpacePos">The position of the floor</param>
    public void AddFloor(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (_gridPos.x < 0 || _gridPos.x > GridSize.x - 1 || _gridPos.y < 0 || _gridPos.y > GridSize.y - 1 || _gridPos.z < 0 || _gridPos.z > GridSize.z - 1) {
            ResizeGrid(_gridPos);
            //Re-adjust from offset from resizing grid
            if (_gridPos.x < 0) _gridPos.x = 0;
            if (_gridPos.y < 0) _gridPos.y = 0;
            if (_gridPos.z < 0) _gridPos.z = 0;
        }

        grid[_gridPos.x, _gridPos.y, _gridPos.z].Floor = true;
    }

    /// <summary>
    /// Checks if there is an existing floor at a position in this system
    /// </summary>
    /// <param name="_worldSpacePos">The position to check for a floor</param>
    /// <returns>Returns true it a floor exists in the position</returns>
    public bool CheckOverlapFloor(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (!CheckPosInGrid(_gridPos))
            return false;
        else
            return grid[_gridPos.x, _gridPos.y, _gridPos.z].Floor == true;
    }

    /// <summary>
    /// Removes a floor at a position. Returns the number of structures it was next to/connected to
    /// </summary>
    /// <param name="_worldSpacePos">The position to remove the floor at</param>
    /// <returns>Integer number of the connections at the position</returns>
    public int RemoveFloor(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);
        int _connections = 0;
        
        for (int i = 0; i < 6; i++) {
            if (CheckGridPosForStructure(_gridPos + traverseDir[i])) {
                _connections++;
            }
        }

        grid[_gridPos.x, _gridPos.y, _gridPos.z].Floor = false;
        return _connections;
    }

    /// <summary>
    /// Adds a floor to the grid at the given position
    /// </summary>
    /// <param name="_worldSpacePos">The position of the floor</param>
    /// <param name="_edge">The type of edge to add</param>
    public void AddEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (_gridPos.x < 0 || _gridPos.x > GridSize.x - 1 || _gridPos.y < 0 || _gridPos.y > GridSize.y - 1 || _gridPos.z < 0 || _gridPos.z > GridSize.z - 1) {
            ResizeGrid(_gridPos);
            //Re-adjust from offset from resizing grid
            if (_gridPos.x < 0) _gridPos.x = 0;
            if (_gridPos.y < 0) _gridPos.y = 0;
            if (_gridPos.z < 0) _gridPos.z = 0;
        }

        grid[_gridPos.x, _gridPos.y, _gridPos.z].GetEdge(_edge) = true;
    }

    /// <summary>
    /// Checks if there is an existing edge at a position in this system
    /// </summary>
    /// <param name="_worldSpacePos">The position to check for a specific edge</param>
    /// <returns>Returns true it the edge exists in the position</returns>
    public bool CheckOverlapEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (!CheckPosInGrid(_gridPos))
            return false;
        else
            return grid[_gridPos.x, _gridPos.y, _gridPos.z].GetEdge(_edge) == true;
    }

    /// <summary>
    /// Removes an edge at a position. Returns the number of structures it was next to/connected to
    /// </summary>
    /// <param name="_worldSpacePos">The position to remove the edge at</param>
    /// <param name="_edge">The type of edge to remove</param>
    /// <returns>Integer number of the connections at the position</returns>
    public int RemoveEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);
        int _connections = 0;

        for (int i = 0; i < 6; i++) {
            if (CheckGridPosForStructure(_gridPos + traverseDir[i])) {
                _connections++;
            }
        }

        grid[_gridPos.x, _gridPos.y, _gridPos.z].GetEdge(_edge) = false;
        return _connections;
    }

    /// <summary>
    /// Turns the world space position into local 3D grid coordinates
    /// </summary>
    /// <param name="_worldSpacePos">The world space position</param>
    /// <returns>A vector3 of integer indexes</returns>
    Vector3Int GetGridPos(Vector3 _worldSpacePos)
    {
        return new Vector3Int(
            (int)((_worldSpacePos.x - OriginWorldPos.x) / gridUnit),
            (int)((_worldSpacePos.y - OriginWorldPos.y) / gridUnit),
            (int)((_worldSpacePos.z - OriginWorldPos.z) / gridUnit));
    }

    /// <summary>
    /// Resizes the grid depending on the new grid location that is added
    /// </summary>
    /// <param name="_gridPos">The grid position where a structure is added</param>
    void ResizeGrid(Vector3Int _gridPos)
    {
        GridStructure[,,] _tempGrid = grid;
        Vector3Int _newSize = GridSize;
        Vector3Int _offset = Vector3Int.zero;

        //X position is out of bounds of current grid
        if (_gridPos.x > GridSize.x - 1)
            _newSize.x++;
        //Placed at -1 from origin
        else if (_gridPos.x < 0) {
            _newSize.x++;
            _offset.x = 1;
        }

        //Y position is out of bounds of current grid
        if (_gridPos.y > GridSize.y - 1)
            _newSize.y++;
        //Placed at -1 from origin
        else if (_gridPos.y < 0) {
            _newSize.y++;
            _offset.y = 1;
        }

        //Z position is out of bounds of current grid
        if (_gridPos.z > GridSize.z - 1)
            _newSize.z++;
        //Placed at -1 from origin
        else if (_gridPos.z < 0) {
            _newSize.z++;
            _offset.z = 1;
        }

        Vector3Int _oldSize = GridSize;
        CreateGrid(_newSize, OriginWorldPos - (_offset * gridUnit));
        CombineGrid(_tempGrid, _oldSize, _offset);
    }

    /// <summary>
    /// Adds another grid to the current grid of this system
    /// </summary>
    /// <param name="_other">The other grid</param>
    /// <param name="_loopBounds">The dimension to loop over</param>
    /// <param name="_offset">The offset origin position of the other grid to the origin on the current grid</param>
    public void CombineGrid(GridStructure[,,] _other, Vector3Int _loopBounds, Vector3Int _offset)
    {
        //Replace grid to new grid with offsets
        for (int x = 0; x < _loopBounds.x; x++) {
            for (int y = 0; y < _loopBounds.y; y++) {
                for (int z = 0; z < _loopBounds.z; z++) {
                    grid[x + _offset.x, y + _offset.y, z + _offset.z] = _other[x, y, z];
                }
            }
        }
    }

    /// <summary>
    /// Splits this system into multiple systems given a split point
    /// </summary>
    /// <param name="_splitPoint">The world space position to split from</param>
    /// <returns>Returns a list of systems that has been split</returns>
    public List<StructureSystem> SplitSystem(Vector3 _splitPoint)
    {
        List<StructureSystem> _systems = new List<StructureSystem>();
        bool[,,] _traversed = new bool[GridSize.x, GridSize.y, GridSize.z];
        bool[,,] _added = new bool[GridSize.x, GridSize.y, GridSize.z];
        Vector3Int _gridSplitPos = GetGridPos(_splitPoint);
        Vector3 _minPoint = Vector3.zero;
        Vector3 _maxPoint = Vector3.zero;

        //Marks grids as traversed, return true when traversed closed system
        bool Traverse(Vector3Int _pos)
        {
            //Track lowest point on grid reached for origin point
            _minPoint = Vector3.Min(_pos, _minPoint);
            //Track highest point on grid to find distance/size of grid
            _maxPoint = Vector3.Max(_pos, _maxPoint);

            _traversed[_pos.x, _pos.y, _pos.z] = true;

            for (int i = 0; i < 6; i++) {
                Vector3Int _traversePos = _pos + traverseDir[i];
                if (!CheckPosInGrid(_traversePos)) {
                    //Skip if pos not in grid
                    continue;
                }
                if (!CheckGridPosForStructure(_traversePos)) {
                    //Skip if pos is not on structure
                    continue;
                }

                if (!_traversed[_traversePos.x, _traversePos.y, _traversePos.z]) {
                    Traverse(_traversePos);
                }
            }

            return true;
        }

        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                for (int z = 0; z < GridSize.z; z++) {
                    //Go to next structure grid
                    if (!_traversed[x, y, z]) {
                        if (CheckGridPosForStructure(new Vector3Int(x, y, z))) {
                            //Reset point trackers for next grid
                            _minPoint = new Vector3(x, y, z);
                            _maxPoint = new Vector3(x, y, z);

                            //Traverse closed system and mark as traversed
                            if (Traverse(new Vector3Int(x, y, z))) {
                                //Offset from old grid
                                Vector3 _origin = OriginWorldPos + (_minPoint * gridUnit);
                                Vector3Int _size = Vector3Int.RoundToInt((_maxPoint + Vector3Int.one) - _minPoint);
                                //Create new system with traversed at x,y,z origin
                                StructureSystem _newSystem = new StructureSystem(_origin, _size, gridUnit);

                                //Add content to grid
                                for (int _x = (int)_minPoint.x; _x < _maxPoint.x + 1; _x++) {
                                    for (int _y = (int)_minPoint.y; _y < _maxPoint.y + 1; _y++) {
                                        for (int _z = (int)_minPoint.z; _z < _maxPoint.z + 1; _z++) {
                                            if (_traversed[_x, _y, _z] && !_added[_x, _y, _z]) {
                                                _newSystem.SetGridPos(new Vector3Int(_x, _y, _z) - Vector3Int.RoundToInt(_minPoint), grid[_x, _y, _z]);
                                                _added[_x, _y, _z] = true;
                                            }
                                        }
                                    }
                                }

                                _systems.Add(_newSystem);

                                Debug.Log("size: " + _size);
                            }
                        }
                    }
                }
            }
        }

        return _systems;
    }
}
