using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlantSO : ScriptableObject
{
    public Sprite Sprite;
    public float SecondsToGrow;
    public int XP;
    public int ResourceCount;
    public PlantComponent Prefab;
    public bool IsCollectable;
}
