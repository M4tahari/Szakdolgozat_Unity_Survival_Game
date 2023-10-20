using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Persistance
{
    void LoadData(WorldState worldState, PlayerState playerState);
    void SaveData(ref WorldState worldState, ref PlayerState playerState);
}
