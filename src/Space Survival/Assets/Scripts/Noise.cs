using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise 
{
    public static float Perlin3D(float _x, float _y, float _z, float _scale)
    {
        float _ab = Mathf.PerlinNoise((_x + 2.3f) * _scale, (_y + 4.2f) * _scale);
        float _bc = Mathf.PerlinNoise((_y + 3.1f) * _scale, (_z + 2.9f) * _scale);
        float _ac = Mathf.PerlinNoise((_x + 1.4f) * _scale, (_z + 3.3f) * _scale);
        float _ba = Mathf.PerlinNoise((_y + 3.9f) * _scale, (_x + 1.9f) * _scale);
        float _cb = Mathf.PerlinNoise((_z + 5.2f) * _scale, (_y + 2.4f) * _scale);
        float _ca = Mathf.PerlinNoise((_z + 2.4f) * _scale, (_x + 3.1f) * _scale);

        float _abc = _ab + _bc + _ac + _ba + _cb + _ca;
        return _abc / 6f;
    }
}
