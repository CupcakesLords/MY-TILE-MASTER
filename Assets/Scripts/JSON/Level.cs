using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class L
{
    public List<float> p;
    public List<List<float>> t;
}

[System.Serializable]
public class Level
{
    public List<float> p;
    public List<L> l; 
}

