using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvasScaler : MonoBehaviour
{
    [SerializeField] Transform target;
    const float ratio = 1.7777f;

    void Start()
    {
        target.localScale = Vector3.one * (Camera.main.aspect / ratio);
    }
}
