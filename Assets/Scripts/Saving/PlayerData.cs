using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;
    public long lastupdated;
    public int level;
    public float currentScore;
    public float sliderHeight;
    public HouseLocationRotation houseLocationRotations;
}


[System.Serializable]
public struct HouseLocationRotation
{
    public List<Vector3> houseLocation;
    public List<Vector3> houseRotation;
}




