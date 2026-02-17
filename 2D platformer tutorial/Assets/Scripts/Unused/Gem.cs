using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Iitem
{
    public void collect()
    {
        Destroy(gameObject);
    }

}
