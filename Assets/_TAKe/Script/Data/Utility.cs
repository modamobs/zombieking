using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static List<int> ShuffleArray(List<int> array, int seed)
    {
        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Count - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Count);
            int tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}