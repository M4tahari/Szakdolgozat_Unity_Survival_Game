using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite sprite;
    public int currentAmount = 1;
    public int stackAmount;
}
