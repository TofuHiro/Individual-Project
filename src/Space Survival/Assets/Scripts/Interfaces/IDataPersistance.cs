using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance
{
    void SaveData(ref GameData _data);

    void LoadData(GameData _data);
}
