using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    [Tooltip("The outline object to toggle active state")]
    [SerializeField] GameObject outlineObject;

    public void ShowOutline()
    {
        outlineObject.SetActive(true);
    }

    public void HideOutline()
    {
        outlineObject.SetActive(false);
    }
}
