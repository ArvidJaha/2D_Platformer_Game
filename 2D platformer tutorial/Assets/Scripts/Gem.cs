using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Iitem
{
    public static event Action<int> OnGemCollect;
    public int worth = 5;
    public void collect()
    {
        OnGemCollect.Invoke(worth);
        Destroy(gameObject);
    }

}
