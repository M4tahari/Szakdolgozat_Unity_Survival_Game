using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Persistance
{
    void LoadData(WorldState worldState);
    void SaveData(ref WorldState worldState);
}
