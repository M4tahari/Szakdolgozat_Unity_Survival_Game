using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class RandomExtensions
{
    public static float NextSingle(this Random random, float min = 0f, float max = 1f)
    {
        return Mathf.Lerp(min, max, (float)random.NextDouble());
    }
}
