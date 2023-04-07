using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class VoxelClump : MonoBehaviour
{
    [Tooltip("For testing. Whether to continuesly update the mesh")]
    [SerializeField] bool updateGeneration;

    [Header("Texture")]
    [SerializeField] string textureName;

    [Header("Size")]
    [Tooltip("The cube size of this chunk")]
    [SerializeField] int chunkSize = 4;
    [Tooltip("The radius where voxels are guranteed to be solid")]
    [SerializeField] float coreRadius = 2f;
    [Tooltip("The distance from the outer radius where the threshold falloff is added from")]
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

    VoxelTextures voxelTextures;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;
    protected bool[,,] voxelMap;

    int vertIndex = 0;

    protected virtual void Start()
    {
        voxelTextures = VoxelTextures.Instance;
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
       
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
        voxelMap = new bool[chunkSize * voxelPerUnit, chunkSize * voxelPerUnit, chunkSize * voxelPerUnit];

        if (useRandomSeed)
            seed = Random.Range(0, 100000);
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;

        transform.position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        PopulateVoxelMap();
        UpdateMeshData();
    }

    void Update()
    {
        if (updateGeneration) {
            PopulateVoxelMap();
            UpdateMeshData();
        }
    }

    /// <summary>
    /// Turns a world space position of the voxel to local 3d coordinates in the voxel map
    /// </summary>
    /// <param name="_worldSpacePos">The world space position of the voxel</param>
    /// <returns>Vector3 coords of the voxel in the voxel map</returns>
    public Vector3 GetVoxel(Vector3 _worldSpacePos)
    {
        float _x = _worldSpacePos.x - (_worldSpacePos.x % (1f / voxelPerUnit));
        float _y = _worldSpacePos.y - (_worldSpacePos.y % (1f / voxelPerUnit));
        float _z = _worldSpacePos.z - (_worldSpacePos.z % (1f / voxelPerUnit));
        return new Vector3(_x, _y, _z) - transform.position;
    }

    /// <summary>
    /// Returns all voxels within a radius from a given world space point
    /// </summary>
    /// <param name="_worldSpacePos">The world space position of the point</param>
    /// <param name="_radius">The radius around the point to get from</param>
    /// <returns></returns>
    public List<Vector3> GetVoxelsInRadius(Vector3 _worldSpacePos, float _radius)
    {
        _worldSpacePos.x -= (_worldSpacePos.x % (1f / voxelPerUnit));
        _worldSpacePos.y -= (_worldSpacePos.y % (1f / voxelPerUnit));
        _worldSpacePos.z -= (_worldSpacePos.z % (1f / voxelPerUnit));

        List<Vector3> _voxelPos = new List<Vector3>();

        for (float x = _worldSpacePos.x - _radius; x < _worldSpacePos.x + _radius; x += (1f / voxelPerUnit)) {
            for (float y = _worldSpacePos.y - _radius; y < _worldSpacePos.y + _radius; y += (1f / voxelPerUnit)) {
                for (float z = _worldSpacePos.z - _radius; z < _worldSpacePos.z + _radius; z += (1f / voxelPerUnit)) {
                    if (CheckVoxel(new Vector3(x, y, z) - transform.position)) {
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
    /// Populates the voxel map using 3D noise
    /// </summary>
    void PopulateVoxelMap()
    {
        voxelMap = new bool[chunkSize * voxelPerUnit, chunkSize * voxelPerUnit, chunkSize * voxelPerUnit];

        for (float x = 0; x < chunkSize; x += 1f / voxelPerUnit) {
            for (float y = 0; y < chunkSize; y += 1f / voxelPerUnit) {
                for (float z = 0; z < chunkSize; z += 1f / voxelPerUnit) {
                    //Start with threshold low for center, higher the further out
                    float _distFromCenter = Vector3.Distance(new Vector3(x, y, z), Vector3.one * (chunkSize / 2));
                    //Rounding from max radius
                    if (_distFromCenter >= chunkSize - (chunkSize - falloffDistance)) 
                        continue;

                    //Solid core from radius
                    if (_distFromCenter < coreRadius)
                        voxelMap[(int)(x * voxelPerUnit), (int)(y * voxelPerUnit), (int)(z * voxelPerUnit)] = true;

                    //Random
                    else {
                        float _frequency = 1;
                        float _amplitude = 1;
                        float _noiseVal;
                        for (int i = 0; i < octaves; i++) {
                            _noiseVal = Noise.Perlin3D(x + seed, y + seed, z + seed, noiseScale * _frequency) * _amplitude;
                            if (_noiseVal > noiseThreshold) {
                                voxelMap[(int)(x * voxelPerUnit), (int)(y * voxelPerUnit), (int)(z * voxelPerUnit)] = true;
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
    /// Recalculates the mesh data based on the voxel map
    /// </summary>
    void UpdateMeshData()
    {
        ClearMeshData();

        for (float x = 0; x < chunkSize; x += 1f / voxelPerUnit) {
            for (float y = 0; y < chunkSize; y += 1f / voxelPerUnit) {
                for (float z = 0; z < chunkSize; z += 1f / voxelPerUnit) {
                    if (voxelMap[(int)(x * voxelPerUnit), (int)(y * voxelPerUnit), (int)(z * voxelPerUnit)]) {
                        AddVoxel(new Vector3(x, y, z));
                    }
                }
            }
        }

        CreateMesh();
    }

    /// <summary>
    /// Adds vertices, triangle and uv data for a voxel at a point
    /// </summary>
    /// <param name="_pos">The 3D index of the voxel in the voxel map</param>
    void AddVoxel(Vector3 _pos)
    {
        for (int i = 0; i < 6; i++) {
            if (!CheckVoxel((VoxelData.faceChecks[i] / voxelPerUnit) + _pos)) {
                vertices.Add((VoxelData.verts[VoxelData.tris[i, 0]] / voxelPerUnit) + _pos);
                vertices.Add((VoxelData.verts[VoxelData.tris[i, 1]] / voxelPerUnit) + _pos);
                vertices.Add((VoxelData.verts[VoxelData.tris[i, 2]] / voxelPerUnit) + _pos);
                vertices.Add((VoxelData.verts[VoxelData.tris[i, 3]] / voxelPerUnit) + _pos);
                //Solid texture, one point on a colour
                uvs.Add(voxelTextures.GetTexture(textureName));
                uvs.Add(voxelTextures.GetTexture(textureName));
                uvs.Add(voxelTextures.GetTexture(textureName));
                uvs.Add(voxelTextures.GetTexture(textureName));

                //Pattern order in voxeldata to exclude duplicates
                triangles.Add(vertIndex);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 2);
                triangles.Add(vertIndex + 1);
                triangles.Add(vertIndex + 3);

                vertIndex += 4;
            }
        }
    }

    /// <summary>
    /// Removes a voxel from the voxel map and updates the mesh
    /// </summary>
    /// <param name="_pos"></param>
    protected void RemoveVoxel(Vector3 _pos)
    {
        voxelMap[(int)(_pos.x * voxelPerUnit), (int)(_pos.y * voxelPerUnit), (int)(_pos.z * voxelPerUnit)] = false;

        UpdateMeshData();
    }

    /// <summary>
    /// Removes voxels from the given list of positions and updates mesh
    /// </summary>
    /// <param name="_positions">List of vector3 positions of voxels</param>
    protected void RemoveVoxels(List<Vector3> _positions)
    {
        foreach (Vector3 _pos in _positions) {
            voxelMap[(int)(_pos.x * voxelPerUnit), (int)(_pos.y * voxelPerUnit), (int)(_pos.z * voxelPerUnit)] = false;
        }

        UpdateMeshData();
    }

    /// <summary>
    /// Returns true if there's a voxel at a given coord in the voxel map
    /// </summary>
    /// <param name="_pos">The 3D coord of the voxel map to check</param>
    /// <returns>Returns true if voxel exists at a coord</returns>
    bool CheckVoxel(Vector3 _pos)
    {
        if ((_pos.x < 0) || (_pos.x > chunkSize - (1f / voxelPerUnit)) || (_pos.y < 0) || (_pos.y > chunkSize - (1f / voxelPerUnit)) || (_pos.z < 0) || (_pos.z > chunkSize - (1f / voxelPerUnit)))
            return false;

        return voxelMap[(int)(_pos.x * voxelPerUnit), (int)(_pos.y * voxelPerUnit), (int)(_pos.z * voxelPerUnit)];
    }

    /// <summary>
    /// Creates the mesh 
    /// </summary>
    void CreateMesh()
    {
        Mesh _mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        if (vertices.Count > 65536) {
            Debug.LogError("Vertices limit bypass by mesh: " + name + " (" + vertices.Count + "/" + "65536)" + ". Reduce the size of this chunk");
        }

        _mesh.RecalculateNormals();
        meshFilter.mesh = _mesh;
        meshCollider.sharedMesh = _mesh;
    }

    /// <summary>
    /// Clears the current mesh data
    /// </summary>
    void ClearMeshData()
    {
        vertIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + (Vector3.one * chunkSize) / 2, Vector3.one * chunkSize);
    }
}
