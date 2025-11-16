using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Environment (0..10)")]
    [SerializeField] private float water = 5f;
    [SerializeField] private float temperature = 5f;

    [Header("Energy System")]
    private float energy = 0f; // 0..5

    [Header("Growth")]
    [SerializeField] private float grow = 0f;           // 0..100
    [SerializeField] private float score = 1f;          // прирост за тик
    [SerializeField] private float secondsPerTick = 1f; // частота роста
    [SerializeField] private float maxGrowMul = 2f;     // максимальный мультипликатор роста
    [SerializeField] private float minGrowMul = 0.1f;   // минимальный мультипликатор роста

    [Header("Stages")]
    [SerializeField] private GameObject[] stages;

    private float growMul = 0f; // 0..maxGrowMul
    private float tickTimer = 0f;

    [Header("Cooldowns")]
    [SerializeField] private float waterCooldown = 1f;
    [SerializeField] private float temperatureCooldown = 1f;

    private float waterTimer = 0f;
    private float temperatureTimer = 0f;

    [Header("Decrease over time")]
    [SerializeField] private float waterDecreaseRate = 0.5f;        // сколько воды теряется за тик
    [SerializeField] private float temperatureDecreaseRate = 0.5f;  // сколько температуры теряется за тик
    [SerializeField] private float waterDecreaseCooldown = 5f;      // время без воды перед уменьшением
    [SerializeField] private float temperatureDecreaseCooldown = 5f;// время без тепла перед уменьшением

    [Header("Leveling")]
    [SerializeField] private int level = 0; // начинается с 0

    void Start()
    {
        ApplyLevelStage();  // включаем нужную стадию сразу
    }

    void Update()
    {
        //---------------------------------------
        // 1. Рассчёт состояния среды (0..1)
        //---------------------------------------
        float waterScore = 1f - Mathf.Abs(water - 5f) / 5f;
        float tempScore = 1f - Mathf.Abs(temperature - 5f) / 5f;

        //---------------------------------------
        // 2. Энергия с плавной кривой
        //---------------------------------------
        energy = 5f * Mathf.Pow((waterScore + tempScore) / 2f, 1.2f);

        //---------------------------------------
        // 3. Мультипликатор роста
        //---------------------------------------
        growMul = Mathf.Max(minGrowMul, (energy / 5f) * maxGrowMul);

        //---------------------------------------
        // 4. Рост по тиковой системе
        //---------------------------------------
        tickTimer += Time.deltaTime;
        if (tickTimer >= secondsPerTick)
        {
            tickTimer -= secondsPerTick;
            grow += score * growMul;

            // проверка на максимум роста
            if (grow >= 100f)
            {
                grow = 0f;
                level += 1;

                if (level < stages.Length)
                    ApplyLevelStage();
                else
                    Debug.LogWarning("Level exceeds stages array!");

                Debug.Log($"Flower leveled up! New level: {level}");
            }

            grow = Mathf.Clamp(grow, 0f, 100f);

            // -----------------------------
            // Уменьшение ресурсов, если нет добавления
            // -----------------------------
            if (waterTimer >= waterDecreaseCooldown)
            {
                water = Mathf.Max(0f, water - waterDecreaseRate);
            }

            if (temperatureTimer >= temperatureDecreaseCooldown)
            {
                temperature = Mathf.Max(0f, temperature - temperatureDecreaseRate);
            }
        }

        //---------------------------------------
        // 5. Таймеры cooldown
        //---------------------------------------
        waterTimer += Time.deltaTime;
        temperatureTimer += Time.deltaTime;
    }

    private void ApplyLevelStage()
    {
        for (int i = 0; i < stages.Length; i++)
            stages[i].SetActive(i == level); // включён только текущий уровень
    }

    // ----------------------------
    // Методы для внешнего контроля с cooldown
    // ----------------------------
    public void AddWater(float amount)
    {
        if (waterTimer >= waterCooldown)
        {
            water = Mathf.Clamp(water + amount, 0f, 10f);
            waterTimer = 0f; // сброс таймера, т.к. вода была добавлена
        }
    }

    public void AddTemperature(float amount)
    {
        if (temperatureTimer >= temperatureCooldown)
        {
            temperature = Mathf.Clamp(temperature + amount, 0f, 10f);
            temperatureTimer = 0f; // сброс таймера, т.к. температура была добавлена
        }
    }

    public void SetWater(float value)
    {
        if (waterTimer >= waterCooldown)
        {
            water = Mathf.Clamp(value, 0f, 10f);
            waterTimer = 0f;
        }
    }

    public void SetTemperature(float value)
    {
        if (temperatureTimer >= temperatureCooldown)
        {
            temperature = Mathf.Clamp(value, 0f, 10f);
            temperatureTimer = 0f;
        }
    }

    // ----------------------------
    // Getters
    // ----------------------------
    public float GetWater() => water;
    public float GetTemperature() => temperature;
    public float GetEnergy() => energy;
    public float GetGrow() => grow;
    public float GetGrowMul() => growMul;
    public int GetLevel() => level;
}
