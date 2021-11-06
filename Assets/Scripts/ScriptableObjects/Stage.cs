using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "Stage")]
public class Stage : ScriptableObject
{
    [Header("Map Data")]
    public new string name;
    public string description;
    public Sprite stageBuilding;
    public int levelID;

    [Header("Dungeon Data")]
    public int floors;
    public int maxRooms;
    // Texture files
    // Furniture prefabs
}
