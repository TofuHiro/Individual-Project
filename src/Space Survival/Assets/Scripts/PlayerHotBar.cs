using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHotBar : MonoBehaviour
{
    #region Singleton
    public static PlayerHotBar Instance;
    void Awake()
    {
        //Singleton init
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
    #endregion


}
