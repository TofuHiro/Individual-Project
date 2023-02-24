using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    [SerializeField] float horizontalSnapOffset;
    [SerializeField] float verticalSnapOffset;
    [SerializeField] float horizontalGridSnap;
    [SerializeField] float verticalGridSnap;

    bool isBluePrint;

    void Update()
    {
        if (isBluePrint) {
            //follow player mouse
        }
    }

    public void SetPosition(Vector3 _pos)
    {
        //transform.position = _pos;
    }

    public void SetRotation(Quaternion _rot)
    {

    }

    public void StartBluePrint()
    {
        PlayerController.OnStartPrimaryAttack += Build;

        isBluePrint = true;
    }

    public void Build()
    {
        PlayerController.OnStartPrimaryAttack -= Build;

        isBluePrint = false;
    }
}
