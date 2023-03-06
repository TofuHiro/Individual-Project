using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public static float Perlin3D(float _x, float _y, float _z)
    {
        float _ab = Mathf.PerlinNoise(_x, _y);
        float _bc = Mathf.PerlinNoise(_y, _z);
        float _ac = Mathf.PerlinNoise(_x, _z);
        float _ba = Mathf.PerlinNoise(_y, _x);
        float _cb = Mathf.PerlinNoise(_z, _y);
        float _ca = Mathf.PerlinNoise(_z, _x);

        float _abc = _ab + _bc + _ac + _ba + _cb + _ca;
        return _abc / 6f;
    }
}
