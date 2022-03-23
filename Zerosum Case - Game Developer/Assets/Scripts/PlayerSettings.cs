using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings")]
public class PlayerSettings : ScriptableObject
{
    public bool isPlaying;
    public bool isBoost;
    public bool isStun;
    public bool isFinish;
    public bool collectCoin;
    public float ForwardSpeed;
    public float sensitivity;
    public float diamondBoostCount;
    public int diamondCount;
    public int level;
    public int coin;
    public int newCoin;
}