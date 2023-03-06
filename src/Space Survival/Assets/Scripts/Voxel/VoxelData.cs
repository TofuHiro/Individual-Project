using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
    //8 vertices in a cube
    public static readonly Vector3[] verts =
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0f, 1f, 1f)
    };

    public static readonly Vector3[] faceChecks =
    {
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 0f),
        new Vector3(0f, 0f, -1f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, -1f, 0f)
    };

    public static readonly int[,] tris =
    {
        //Removed duplicates for optimization
        {5, 6, 4, 7},     //Front face    - {5, 6, 4, 4, 6, 7}
        {1, 2, 5, 6},     //Right face    - {1, 2, 5, 5, 2, 6}
        {0, 3, 1, 2},     //Back face     - {0, 3, 1, 1, 3, 2}
        {4, 7, 0, 3},     //Left face     - {4, 7, 0, 0, 7, 3}
        {3, 7, 2, 6},     //Top face      - {3, 7, 2, 2, 7, 6}
        {1, 5, 0, 4}      //Bottom face   - {1, 5, 0, 0, 5, 4}
    };

    public static readonly Vector2[] uvs =
    {
        //Removed duplicates for optimization
        new Vector2(0f, 0f),    //1
        new Vector2(0f, 1f),    //2
        new Vector2(1f, 0f),    //3
        /*new Vector2(1f, 0f),  //4
        new Vector2(0f, 1f),*/  //5
        new Vector2(1f, 1f)     //6
    };
}
