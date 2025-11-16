using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    [Header("Flower")]
    public float defFlowerWater = 5f;
    public float defFlowerTemp = 5f;

    public float score = 0f;


    [Header("Wind")]
    public float windRepulcion;
    public float windDuration;
    public float minDurNoWind;
    public float maxDurNoWind;

    [Header("Cloud")]
    public float rainWaterPoints;

    public float snopwWaterPoints;
    public float snowTempPoints;

    public float snowDuration;
    public float minDurNoSnow;
    public float maxDurNoSnow;

    [Header("Sun and Moon")]
    public int sunRaysCount;
    public int moonRaysCount;

    public float sunRayTempPoints;
    public float moonRayTempPoints;



}

[CreateAssetMenu(fileName = "LevelsData", menuName = "Game/Levels Data")]
public class LevelsData : ScriptableObject
{
    public LevelConfig[] levels = new LevelConfig[5];
}
