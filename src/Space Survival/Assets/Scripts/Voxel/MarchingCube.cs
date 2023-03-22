using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MarchingCube : MonoBehaviour
{
    [Tooltip("For testing. Whether to continuesly update the mesh")]
    [SerializeField] bool updateGeneration;

    [Header("Size")]
    [Tooltip("The cube size of this chunk")]
    [SerializeField] int cubeSize = 4;
    [Tooltip("The radius where voxels are guranteed to be solid")]
    [SerializeField] float coreRadius = 2f;
    [Tooltip("The radius where voxels are guranteed to be empty")]
    [SerializeField] float falloffDistance = 2f;
    [Tooltip("The number of voxels per unit cube")]
    [SerializeField] protected int voxelPerUnit = 1;

    [Header("Noise Generation")]
    [Tooltip("A seed value to generate")]
    [SerializeField] int seed;
    [Tooltip("Use a randomly generated seed")]
    [SerializeField] bool useRandomSeed;
    [Tooltip("Scale applied when generating a noise map")]
    [SerializeField] float noiseScale = .6f;
    [Tooltip("Values from the noise map below this value will be considered solid")]
    [Range(0f, 1f)]
    [SerializeField] float noiseThreshold = .5f;
    [Tooltip("Controls increase in noise frequency from octaves. Frequency = Lacunarity ^ Octave")]
    [SerializeField] float lacunarity = 2f;
    [Tooltip("The number of octaves to calculate noise frequencies")]
    [SerializeField] int octaves = 3;
    [Range(0, 1)]
    [Tooltip("Controls decrease in noise amplitudes from octaves. Amplitude = Persistance ^ Octave")]
    [SerializeField] float persistance = .5f;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    List<Vector3> vertices;
    List<int> triangles;
    bool[,,] vertMap;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        vertices = new List<Vector3>();
        triangles = new List<int>();

        //+ 1unit, since storing vertices and not voxels themselves
        vertMap = new bool[(cubeSize * voxelPerUnit) + voxelPerUnit, (cubeSize * voxelPerUnit) + voxelPerUnit, (cubeSize * voxelPerUnit) + voxelPerUnit];

        if (useRandomSeed)
            seed = Random.Range(0, 100000);
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;

        PopulateVertMap();
        UpdateMeshData();
    }

    void Update()
    {
        if (updateGeneration) {
            PopulateVertMap();
            UpdateMeshData();
        }
    }

    /// <summary>
    /// Returns the nearest vertex given a world space position
    /// </summary>
    /// <param name="_worldSpacePos">The world space position of the vertex</param>
    /// <returns>The 3D index of the vertex in the vertMap</returns>
    public Vector3 GetVert(Vector3 _worldSpacePos)
    {
        float _x = _worldSpacePos.x - (_worldSpacePos.x % (1f / voxelPerUnit));
        float _y = _worldSpacePos.y - (_worldSpacePos.y % (1f / voxelPerUnit));
        float _z = _worldSpacePos.z - (_worldSpacePos.z % (1f / voxelPerUnit));
        return new Vector3(_x, _y, _z) - transform.position;
    }

    /// <summary>
    /// Returns all vertices around a radius at a given world space position
    /// </summary>
    /// <param name="_worldSpacePos">The world space position to get vertices from</param>
    /// <param name="_radius">The radius around the point to get from</param>
    /// <returns></returns>
    public List<Vector3> GetVertsInRadius(Vector3 _worldSpacePos, float _radius)
    {
        _worldSpacePos.x -= (_worldSpacePos.x % (1f / voxelPerUnit));
        _worldSpacePos.y -= (_worldSpacePos.y % (1f / voxelPerUnit));
        _worldSpacePos.z -= (_worldSpacePos.z % (1f / voxelPerUnit));

        List<Vector3> _voxelPos = new List<Vector3>();

        for (float x = _worldSpacePos.x - _radius; x < _worldSpacePos.x + _radius; x += (1f / voxelPerUnit)) {
            for (float y = _worldSpacePos.y - _radius; y < _worldSpacePos.y + _radius; y += (1f / voxelPerUnit)) {
                for (float z = _worldSpacePos.z - _radius; z < _worldSpacePos.z + _radius; z += (1f / voxelPerUnit)) {
                    if (CheckVert(new Vector3(x, y, z) - transform.position)) {
                        if ((Vector3.Distance(new Vector3(x, y, z), _worldSpacePos) <= _radius)) {
                            _voxelPos.Add(new Vector3(x, y, z) - transform.position);
                        }
                    }
                }
            }
        }

        return _voxelPos;
    }

    /// <summary>
    /// Populates the verts map using 3D noise
    /// </summary>
    void PopulateVertMap()
    {
        vertMap = new bool[(cubeSize * voxelPerUnit) + voxelPerUnit, (cubeSize * voxelPerUnit) + voxelPerUnit, (cubeSize * voxelPerUnit) + voxelPerUnit];

        for (float x = 0; x < cubeSize + 1; x += 1f / voxelPerUnit) {
            for (float y = 0; y < cubeSize + 1; y += 1f / voxelPerUnit) {
                for (float z = 0; z < cubeSize + 1; z += 1f / voxelPerUnit) {
                    //Start with threshold low for center, higher the further out
                    float _distFromCenter = Vector3.Distance(new Vector3(x, y, z), Vector3.one * (cubeSize / 2));
                    //Rounding from max radius
                    if (_distFromCenter >= cubeSize - (cubeSize - falloffDistance)) 
                        continue;
                    
                    //Solid core from radius
                    if (_distFromCenter < coreRadius) 
                        vertMap[(int)(x * voxelPerUnit), (int)(y * voxelPerUnit), (int)(z * voxelPerUnit)] = true;
                    
                    else {
                        float _frequency = 1;
                        float _amplitude = 1;
                        float _noiseVal;
                        for (int i = 0; i < octaves; i++) {
                            _noiseVal = Noise.Perlin3D(x + seed, y + seed, z + seed, noiseScale * _frequency) * _amplitude;
                            if (_noiseVal > noiseThreshold) {
                                vertMap[(int)(x * voxelPerUnit), (int)(y * voxelPerUnit), (int)(z * voxelPerUnit)] = true;
                            }
                            _frequency *= lacunarity;
                            _amplitude *= persistance;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes a vertex from the verts map and updates mesh
    /// </summary>
    /// <param name="_pos"></param>
    protected void RemoveVert(Vector3 _pos)
    {
        vertMap[(int)(_pos.x * voxelPerUnit), (int)(_pos.y * voxelPerUnit), (int)(_pos.z * voxelPerUnit)] = false;

        UpdateMeshData();
    }

    /// <summary>
    /// Removes vertices from the list of given positions and updates mesh
    /// </summary>
    /// <param name="_positions">List of vector3 positions to remove vertices at</param>
    protected void RemoveVerts(List<Vector3> _positions)
    {
        foreach (Vector3 _pos in _positions) {
            vertMap[(int)(_pos.x * voxelPerUnit), (int)(_pos.y * voxelPerUnit), (int)(_pos.z * voxelPerUnit)] = false;
        }

        UpdateMeshData();
    }
    
    /// <summary>
    /// Check if a vertex exists at a given position
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns>Return true if a vertex exists at the given pos</returns>
    bool CheckVert(Vector3 _pos)
    {
        if ((_pos.x < 0) || (_pos.x > cubeSize - (1f / voxelPerUnit)) || (_pos.y < 0) || (_pos.y > cubeSize - (1f / voxelPerUnit)) || (_pos.z < 0) || (_pos.z > cubeSize - (1f / voxelPerUnit)))
            return false;

        return vertMap[(int)(_pos.x * voxelPerUnit), (int)(_pos.y * voxelPerUnit), (int)(_pos.z * voxelPerUnit)];
    }

    /// <summary>
    /// Recalculates the mesh data based on the verts map
    /// </summary>
    void UpdateMeshData()
    {
        ClearMeshData();

        //Loop through each "cube/space" in the vertMap (so without + 1)
        for (float x = 0; x < cubeSize; x += 1f / voxelPerUnit) {
            for (float y = 0; y < cubeSize; y += 1f / voxelPerUnit) {
                for (float z = 0; z < cubeSize; z += 1f / voxelPerUnit) {
                    bool[] _cubeVerts = new bool[8];
                    //Loop through each corner of the current cube
                    for (int i = 0; i < 8; i++) {
                        Vector3 _vert = new Vector3(x, y, z) + (MarchingCubeData.verts[i] / voxelPerUnit);
                        //Assign active corners in cube
                        _cubeVerts[i] = vertMap[(int)(_vert.x * voxelPerUnit), (int)(_vert.y * voxelPerUnit), (int)(_vert.z* voxelPerUnit)];
                    }
                    //March cube with active corners for current cube
                    MarchCube(new Vector3(x, y, z), _cubeVerts);
                }
            }
        }

        CreateMesh();
    }

    /// <summary>
    /// Add verts based on the given cube configuration
    /// </summary>
    /// <param name="_pos">The position of the cube vertices</param>
    /// <param name="_cubeVerts">The vertices of the cube at the given position</param>
    void MarchCube(Vector3 _pos, bool[] _cubeVerts)
    {
        int _configIndex = GetConfiguration(_cubeVerts);
        
        //Config 0 and 255 are completely emtpy/full respectively, ignore
        if (_configIndex == 0 || _configIndex == 255)
            return;

        //(Max 5 tris in a cube) * (3 Verts in a triangle) = loop 15 times
        for (int i = 0; i < 15; i++) {
            int _index = MarchingCubeData.tris[_configIndex, i];
            //Termination index in tris array
            if (_index == -1)
                return;

            //Start and end verts of an edge
            Vector3 _startVert = _pos + (MarchingCubeData.edges[_index, 0] / voxelPerUnit);
            Vector3 _endVert = _pos + (MarchingCubeData.edges[_index, 1] / voxelPerUnit);

            //Midpoint
            Vector3 _vertPos = (_startVert + _endVert) / 2f;

            vertices.Add(_vertPos);
            //Index of added vertex
            triangles.Add(vertices.Count - 1);
        }
    }

    /// <summary>
    /// Get the configuration from triangles table based on the current active vertices
    /// </summary>
    /// <param name="_cubeVerts">The vertices of a cube</param>
    /// <returns></returns>
    int GetConfiguration(bool[] _cubeVerts)
    {
        int _configurationIndex = 0;
        //8 bit, since 255 max configs
        for (int i = 0; i < 8; i++) {
            //Bit manipulation, if corner exists, set as true, which gives a final 8 bit value used to look up the ordered tris table
            if (_cubeVerts[i] == true)
                _configurationIndex |= 1 << i;
        }
        //Tris table is symmetric where each end is the same corners but inverted from inside-out/outside-inwards
        //How the marching cube is generated, without inverting, the cubes normals are inverted facing inwards
        return 255 - _configurationIndex;
    }

    /// <summary>
    /// Creates the mesh
    /// </summary>
    void CreateMesh()
    {
        Mesh _mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray()
        };
       
        _mesh.RecalculateNormals();
        meshFilter.mesh = _mesh;
        meshCollider.sharedMesh = _mesh;
    }

    void ClearMeshData()
    {
        vertices.Clear();
        triangles.Clear();
    }
}
