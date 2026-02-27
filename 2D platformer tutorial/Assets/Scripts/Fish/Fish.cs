using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour, Iitem
{
    public static event Action<int> OnFishCollected;
    public int value = 1;
    bool collected = false;


    public void collect()
    {
        if (collected) return;
        collected = true;

        OnFishCollected.Invoke(value);
        Destroy(gameObject);
    }

}
