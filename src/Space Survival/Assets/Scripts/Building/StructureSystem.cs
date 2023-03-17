using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureSystem
{
    readonly Vector3Int[] traverseDir =
    {
        new Vector3Int(0, -1, 0),   //Down
        new Vector3Int(0, 1, 0),    //Up
        new Vector3Int(0, 0, 1),    //Front
        new Vector3Int(1, 0, 0),    //Right
        new Vector3Int(0, 0, -1),   //Back
        new Vector3Int(-1, 0, 0),   //Left
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
    /// Returns the size of the grid excluding empty rows/columns
    /// </summary>
    /// <returns>The size in a Vector3Int</returns>
    Vector3Int GetStructureSize()
    {
        Vector3Int _min = GridSize;
        Vector3Int _max = Vector3Int.zero;
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                for (int z = 0; z < GridSize.z; z++) {
                    if (CheckGridPosForStructure(new Vector3Int(x, y, z))) {
                        _min = Vector3Int.Min(_min, new Vector3Int(x, y, z));
                        _max = Vector3Int.Max(_max, new Vector3Int(x, y, z));
                    }
                }
            }
        }
        return _max + Vector3Int.one - _min;
    }

    /// <summary>
    /// Turns the world space position into local 3D grid coordinates
    /// </summary>
    /// <param name="_worldSpacePos">The world space position</param>
    /// <returns>Indexes in a Vector3Int</returns>
    Vector3Int GetGridPos(Vector3 _worldSpacePos)
    {
        return new Vector3Int(
            (int)((_worldSpacePos.x - OriginWorldPos.x) / gridUnit),
            (int)((_worldSpacePos.y - OriginWorldPos.y) / gridUnit),
            (int)((_worldSpacePos.z - OriginWorldPos.z) / gridUnit));
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
    /// Resizes the grid to the given size
    /// </summary>
    /// <param name="_newSize">The new size to resize to</param>
    /// <param name="_offset">The offset to apply to the contents from the old grid size</param>
    void ResizeGrid(Vector3Int _newSize, Vector3Int _offset)
    {
        GridStructure[,,] _tempGrid = grid;
        Vector3Int _oldSize = GridSize;
        CreateGrid(_newSize, OriginWorldPos - (_offset * gridUnit));
        CombineGrid(_tempGrid, _oldSize, _offset);
    }

    /// <summary>
    /// Adds another grid's content to the current grid of this system
    /// </summary>
    /// <param name="_other">The other grid to add</param>
    /// <param name="_upperLoopBounds">The upper bound to loop to</param>
    /// <param name="_offset">The offset to apply when adding the new grid contents</param>
    public void CombineGrid(GridStructure[,,] _other, Vector3Int _upperLoopBounds, Vector3Int _offset)
    {
        //Replace grid to new grid with offsets
        for (int x = 0; x < _upperLoopBounds.x; x++) {
            for (int y = 0; y < _upperLoopBounds.y; y++) {
                for (int z = 0; z < _upperLoopBounds.z; z++) {
                    if (CheckPosInGrid(new Vector3Int(x + _offset.x, y + _offset.y, z + _offset.z))) {
                        grid[x + _offset.x, y + _offset.y, z + _offset.z] = _other[x, y, z];
                    }
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
    /// Checks if any structure is built on at an index in the grid
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

    /// <summary>
    /// Checks if any structure is built on at an index in the grid
    /// </summary>
    /// <param name="_gridPos">The grid index to check</param>
    /// <returns>Returns true if any structure exists on the given position</returns>
    bool CheckGridPosForStructure(Vector3Int _gridPos)
    {
        if (!CheckPosInGrid(_gridPos))
            return false;
        else
            return !grid[_gridPos.x, _gridPos.y, _gridPos.z].IsEmpty;
    }

    /// <summary>
    /// Checks if an index is within the current grid space or not
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
    /// Checks if a structure is built on the grid at a given direction
    /// </summary>
    /// <param name="_gridPos">The grid index to check</param>
    /// <param name="_traverseDirIndex">The direction of the edge on the grid to check</param>
    /// <returns>Returns true if a structure exists</returns>
    bool CheckStructureAtDir(Vector3Int _gridPos, int _traverseDirIndex)
    {
        //Down
        if (_traverseDirIndex == 0) {
            return CheckOverlapFloor(_gridPos);
        }
        //Up
        else if (_traverseDirIndex == 1) {
            return CheckOverlapFloor(_gridPos + new Vector3Int(0, 1, 0));
        }
        //Front
        else if (_traverseDirIndex == 2) {
            return CheckOverlapEdge(_gridPos, Edge.Front);
        }
        //Right
        else if (_traverseDirIndex == 3) {
            return CheckOverlapEdge(_gridPos, Edge.Right);
        }
        //Back
        else if (_traverseDirIndex == 4) {
            return CheckOverlapEdge(_gridPos, Edge.Back);
        }
        //Left
        else { //(_traverseDirIndex == 5)
            return CheckOverlapEdge(_gridPos, Edge.Left);
        }
    }

    /// <summary>
    /// Checks if a structure is built on a grid given a direction from another grid
    /// </summary>
    /// <param name="_gridPos">The grid index to check</param>
    /// <param name="_traverseFromDir">The incoming direction of the edge from the other grid to check</param>
    /// <returns>Returns true if a structure exists</returns>
    bool CheckStructureFromDir(Vector3Int _gridPos, int _traverseFromDir)
    {
        //Dir is Down, coming from above, Check Top
        if (_traverseFromDir == 0) {
            return CheckOverlapFloor(_gridPos + new Vector3Int(0, 1, 0));
        }
        //Dir is Up, coming from below, Check Bot
        else if (_traverseFromDir == 1) {
            return CheckOverlapFloor(_gridPos);
        }
        //Dir is Front, coming from behind, Check Back
        else if (_traverseFromDir == 2) {
            return CheckOverlapEdge(_gridPos, Edge.Back);
        }
        //Dir is Right, coming from Left, Check Left
        else if (_traverseFromDir == 3) {
            return CheckOverlapEdge(_gridPos, Edge.Left);
        }
        //Dir is Back, coming from Front, Check Front
        else if (_traverseFromDir == 4) {
            return CheckOverlapEdge(_gridPos, Edge.Front);
        }
        //Dir is Left, coming from Right, Check Right
        else { //(_traverseFromDir == 5)
            return CheckOverlapEdge(_gridPos, Edge.Right);
        }
    }

    /// <summary>
    /// Adds a floor to the grid at the given position
    /// </summary>
    /// <param name="_worldSpacePos">The world space position of the floor</param>
    public void AddFloor(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (_gridPos.x < 0 || _gridPos.x > GridSize.x - 1 || _gridPos.y < 0 || _gridPos.y > GridSize.y - 1 || _gridPos.z < 0 || _gridPos.z > GridSize.z - 1) {
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

            ResizeGrid(_newSize, _offset);
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
    /// <returns>Returns true if a floor exists in the position</returns>
    public bool CheckOverlapFloor(Vector3 _worldSpacePos)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (!CheckPosInGrid(_gridPos))
            return false;
        else
            return grid[_gridPos.x, _gridPos.y, _gridPos.z].Floor == true;
    }

    /// <summary>
    /// Checks if there is an existing floor at a grid index in this system
    /// </summary>
    /// <param name="_gridPos">The grid index to check for a floor</param>
    /// <returns>Returns true it a floor exists in the position</returns>
    bool CheckOverlapFloor(Vector3Int _gridPos)
    {
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

        Vector3Int _newSize = GetStructureSize();
        if (_newSize.magnitude < GridSize.magnitude) {
            Vector3Int _offset = Vector3Int.zero;
            if (_gridPos.x == 0 && _newSize.x < GridSize.x) 
                _offset.x = -1;
            if (_gridPos.y == 0 && _newSize.y < GridSize.y)
                _offset.y = -1;
            if (_gridPos.z == 0 && _newSize.z < GridSize.z)
                _offset.z = -1;
            ResizeGrid(_newSize, _offset);
        }

        return _connections;
    }

    /// <summary>
    /// Adds an edge to the grid at the given position
    /// </summary>
    /// <param name="_worldSpacePos">The position of the edge</param>
    /// <param name="_edge">The type of edge to add</param>
    public void AddEdge(Vector3 _worldSpacePos, Edge _edge)
    {
        Vector3Int _gridPos = GetGridPos(_worldSpacePos);

        if (_gridPos.x < 0 || _gridPos.x > GridSize.x - 1 || _gridPos.y < 0 || _gridPos.y > GridSize.y - 1 || _gridPos.z < 0 || _gridPos.z > GridSize.z - 1) {
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

            ResizeGrid(_newSize, _offset);
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
    /// <param name="_edge">The edge to check for</param>
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
    /// Checks if there is an existing edge at a grid index in this system
    /// </summary>
    /// <param name="_gridPos">The grid index to check for a specific edge</param>
    /// <param name="_edge">The edge to check for</param>
    /// <returns>Returns true it the edge exists at the index</returns>
    bool CheckOverlapEdge(Vector3Int _gridPos, Edge _edge)
    {
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

        Vector3Int _newSize = GetStructureSize();
        if (_newSize.magnitude < GridSize.magnitude) {
            Vector3Int _offset = Vector3Int.zero;
            if (_gridPos.x == 0 && _newSize.x < GridSize.x)
                _offset.x = -1;
            if (_gridPos.y == 0 && _newSize.y < GridSize.y)
                _offset.y = -1;
            if (_gridPos.z == 0 && _newSize.z < GridSize.z)
                _offset.z = -1;
            ResizeGrid(_newSize, _offset);
        }

        return _connections;
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
        //Track grids that has been added to a system
        bool[,,] _added = new bool[GridSize.x, GridSize.y, GridSize.z];
        Vector3Int _gridSplitPos = GetGridPos(_splitPoint);
        Vector3 _minPoint = Vector3.zero;
        Vector3 _maxPoint = Vector3.zero;

        //Marks grids as traversed, return true when traversed closed system
        void Traverse(Vector3Int _pos)
        {
            //Track lowest point on grid reached for origin point
            _minPoint = Vector3.Min(_pos, _minPoint);
            //Track highest point on grid to find size of grid
            _maxPoint = Vector3.Max(_pos, _maxPoint);

            _traversed[_pos.x, _pos.y, _pos.z] = true;

            for (int i = 0; i < 6; i++) {
                Vector3Int _traversePos = _pos + traverseDir[i];
                //Skip if pos not in grid
                if (!CheckPosInGrid(_traversePos)) 
                    continue;
                //Skip if pos is not on structure
                if (!CheckGridPosForStructure(_traversePos)) 
                    continue;
                
                if (!_traversed[_traversePos.x, _traversePos.y, _traversePos.z]) {
                    Traverse(_traversePos);
                }
            }
        }

        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                for (int z = 0; z < GridSize.z; z++) {
                    //Go to next grid structure
                    if (!CheckGridPosForStructure(new Vector3Int(x, y, z)))
                        continue;
                    if (_traversed[x, y, z])
                        continue;

                    //Reset point trackers for next grid
                    _minPoint = new Vector3(x, y, z);
                    _maxPoint = new Vector3(x, y, z);

                    //Traverse closed system and mark as traversed
                    Traverse(new Vector3Int(x, y, z));

                    //Offset from old grid
                    Vector3 _origin = OriginWorldPos + (_minPoint * gridUnit);
                    Vector3Int _size = Vector3Int.RoundToInt((_maxPoint + Vector3Int.one) - _minPoint);
                    //Create new system with traversed grids
                    StructureSystem _newSystem = new StructureSystem(_origin, _size, gridUnit);

                    //Add content to grid
                    for (int _x = (int)_minPoint.x; _x < _maxPoint.x + 1; _x++) {
                        for (int _y = (int)_minPoint.y; _y < _maxPoint.y + 1; _y++) {
                            for (int _z = (int)_minPoint.z; _z < _maxPoint.z + 1; _z++) {
                                //Add traversed and not already added grids
                                if (_traversed[_x, _y, _z] && !_added[_x, _y, _z]) {
                                    _newSystem.SetGridPos(new Vector3Int(_x, _y, _z) - Vector3Int.RoundToInt(_minPoint), grid[_x, _y, _z]);
                                    _added[_x, _y, _z] = true;
                                }
                            }
                        }
                    }
                    _systems.Add(_newSystem);
                }
            }
        }
        return _systems;
    }

    /// <summary>
    /// Checks and marks all grids as sealed or unsealed
    /// </summary>
    public void CheckSealed()
    {
        bool[,,] _traversed = new bool[GridSize.x, GridSize.y, GridSize.z];
        bool[,,] _sealCheck = new bool[GridSize.x, GridSize.y, GridSize.z];
        bool[,,] _voidCheck = new bool[GridSize.x, GridSize.y, GridSize.z];
        Vector3Int _breachPoint = Vector3Int.zero;

        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                for (int z = 0; z < GridSize.z; z++) {
                    //Find next structure unit that hasnt been checked
                    if (CheckGridPosForStructure(new Vector3Int(x, y, z))) {
                        if (!_traversed[x, y, z]) {
                            //If no breach found
                            if (Traverse(new Vector3Int(x, y, z))) 
                                Seal(new Vector3Int(x, y, z));
                            //Else breach found
                            else 
                                Unseal(new Vector3Int(x, y, z), _breachPoint);
                        }
                    }
                }
            }
        }

        //Traverses through system, returns false if breach found
        bool Traverse(Vector3Int _pos)
        {
            _traversed[_pos.x, _pos.y, _pos.z] = true;

            for (int i = 0; i < 6; i++) {
                Vector3Int _traversePos = _pos + traverseDir[i];
                //If no wall/floor blocking direction
                if (!CheckStructureAtDir(_pos, i) && !CheckStructureFromDir(_traversePos, i)) {
                    //If traverse on bounds/edge, return false;
                    if (!CheckPosInGrid(_traversePos)) {
                        _breachPoint = _traversePos;
                        return false;
                    }
                    //If no structure, check if void
                    if (!CheckGridPosForStructure(_traversePos)) {
                        //Reset array for next traverse
                        _voidCheck = new bool[GridSize.x, GridSize.y, GridSize.z];
                        if (CheckForVoid(_traversePos)) {
                            _breachPoint = _traversePos;
                            return false;
                        }
                    }
                    //Return false if other not sealed
                    if (!_traversed[_traversePos.x, _traversePos.y, _traversePos.z]) {
                        if (Traverse(_traversePos) == false)
                            return false;
                    }
                }
            }

            //End
            return true;
        }

        //Marks all grids in a closed system as unsealed
        void Unseal(Vector3Int _startPos, Vector3Int _breachPoint)
        {
            _sealCheck[_startPos.x, _startPos.y, _startPos.z] = true;
            _traversed[_startPos.x, _startPos.y, _startPos.z] = true;
            grid[_startPos.x, _startPos.y, _startPos.z].IsSealed = false;

            for (int i = 0; i < 6; i++) {
                Vector3Int _traverseDir = _startPos + traverseDir[i];
                //If structure dont exist, dont traverse
                if (!CheckGridPosForStructure(_traverseDir))
                    continue;
                //If wall/floor, ignore/no traverse
                if (CheckStructureAtDir(_startPos, i) || CheckStructureFromDir(_traverseDir, i))
                    continue;
                //Ignore breachpoint, no traverse
                if (_traverseDir == _breachPoint)
                    continue;

                if (!_sealCheck[_traverseDir.x, _traverseDir.y, _traverseDir.z]) {
                    Unseal(_traverseDir, _breachPoint);
                }
            }
        }

        //Marks all grids in a closed system as sealed
        void Seal(Vector3Int _startPos)
        {
            _sealCheck[_startPos.x, _startPos.y, _startPos.z] = true;
            _traversed[_startPos.x, _startPos.y, _startPos.z] = true;
            grid[_startPos.x, _startPos.y, _startPos.z].IsSealed = true;

            for (int i = 0; i < 6; i++) {
                Vector3Int _traverseDir = _startPos + traverseDir[i];
                //If structure dont exist, dont traverse
                if (!CheckGridPosForStructure(_traverseDir))
                    continue;
                //If wall/floor, ignore/no traverse
                if (CheckStructureAtDir(_startPos, i) || CheckStructureFromDir(_traverseDir, i))
                    continue;

                if (!_sealCheck[_traverseDir.x, _traverseDir.y, _traverseDir.z]) {
                    Seal(_traverseDir);
                }
            }
        }

        //Traverse through whole grid and find position outside of grid + is not blocked by structure, returns true if void found
        bool CheckForVoid(Vector3Int _startPos)
        {
            _voidCheck[_startPos.x, _startPos.y, _startPos.z] = true;
            //Traverse and try find void
            for (int i = 0; i < 6; i++) {
                Vector3Int _traverseDir = _startPos + traverseDir[i];
                //If wall/floor, ignore/no traverse
                if (CheckStructureAtDir(_startPos, i))
                    continue;
                //Check if blocked by walls from other grid
                if (CheckGridPosForStructure(_traverseDir)) {
                    if (CheckStructureFromDir(_traverseDir, i)) {
                        continue;
                    }
                }
                //Check if grid is void/in grid
                if (!CheckPosInGrid(_traverseDir)) {
                    return true;
                }

                if (!_voidCheck[_startPos.x, _startPos.y, _startPos.z]) {
                    CheckForVoid(_traverseDir);
                }
            }
            return false;
        }

        //Show
        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                for (int z = 0; z < GridSize.z; z++) {
                    Debug.Log(new Vector3Int(x,y,z) + ": " + grid[x,y,z].IsSealed);
                }
            }
        }
    }
}
