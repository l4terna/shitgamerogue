using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Environment (0..10)")]
    [SerializeField] public float water = 5f;
    [SerializeField] public float temperature = 5f;

    [Header("Energy System")]
    private float energy = 0f; // 0..5

    [Header("Growth")]
    [SerializeField] private float grow = 0f;           // 0..100
    [SerializeField] public float score = 1f;          // прирост за тик
    [SerializeField] private float secondsPerTick = 1f; // частота роста
    [SerializeField] private float maxGrowMul = 2f;     // максимальный мультипликатор роста
    [SerializeField] private float minGrowMul = 0.1f;   // минимальный мультипликатор роста

    [Header("Negative Growth")]
    [SerializeField] private float negativeGrowMax = 2f; // максимальное уменьшение
    [SerializeField] private float negativeGrowEdge = 2f; // зона у края шкалы, где начинается уменьшение

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

            // ----------------------------------------------------
            //  NEGATIVE GROW WHEN WATER/TEMP NEAR 0 OR 10
            // ----------------------------------------------------
            float negative = 0f;

            // расстояние до "края" для воды и температуры
            float waterEdgeDist = Mathf.Min(water, 10f - water);
            float tempEdgeDist = Mathf.Min(temperature, 10f - temperature);

            // вычисляем долю близости к краю
            float waterDanger = Mathf.Clamp01((negativeGrowEdge - waterEdgeDist) / negativeGrowEdge);
            float tempDanger = Mathf.Clamp01((negativeGrowEdge - tempEdgeDist) / negativeGrowEdge);

            // выбираем сильнейший штраф
            float danger = Mathf.Max(waterDanger, tempDanger);

            // danger = 0 → норма
            // danger = 1 → точно на 0 или 10
            negative = negativeGrowMax * danger;

            if (negative > 0f)
            {
                grow -= negative;

                // если рост упал ниже нуля — понижаем уровень
                if (grow < 0f)
                {
                    if (level > 0)
                    {
                        level -= 1;
                        ApplyLevelStage();
                        Debug.Log($"Flower level DOWN! New level: {level}");
                        AudioManager.Instance.PlayEffect("down");
                        grow = 50f;
                    }
                    else
                    {
                        OnMinLevelReached(); // заглушка
                    }


                }
            }


            // проверка на максимум роста
            if (grow >= 100f)
            {
                grow = 0f;
                level += 1;

                if (level < stages.Length)
                    ApplyLevelStage();
                else
                {
                    GameManager.Instance.WinGame();
                    return;
                }

                GameManager.Instance.StartLevel(level);

                Debug.Log($"Flower leveled up! New level: {level}");

                AudioManager.Instance.PlayEffect("levelup");
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
    private void OnMinLevelReached()
    {
        GameManager.Instance.EndGame();
    }

    private void ApplyLevelStage()
    {
        for (int i = 0; i < stages.Length; i++)
            stages[i].SetActive(i == level); // включён только текущий уровень
    }

    public void ResetLevel()
    {
        if (GameManager.Instance == null || GameManager.Instance.GetCurrentLevelConfig() == null)
        {
            Debug.LogWarning("Cannot reset level: CurrentLevelConfig is null!");
            return;
        }

        LevelConfig config = GameManager.Instance.GetCurrentLevelConfig();

        // Сброс роста
        grow = 0f;
        growMul = minGrowMul;

        // Сброс ресурсов на значения из LevelConfig
        water = 5;
        temperature = 5;
        energy = 0f;

        // Сброс таймеров
        tickTimer = 0f;
        waterTimer = 0f;
        temperatureTimer = 0f;

        // Сброс уровня
        level = 0;

        // Устанавливаем правильный спрайт/стадию
        ApplyLevelStage();

        Debug.Log($"Flower reset to level 0 with water={water}, temperature={temperature}");
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
