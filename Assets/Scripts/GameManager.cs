using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Flower flower;
    [SerializeField] GameObject windHolder;
    [SerializeField] WeatherSwitcher weatherSwitcher;
    [SerializeField] Rain rain;
    [SerializeField] Snowfall snowfall;
    [SerializeField] CircleSunrays sun;
    [SerializeField] CircleSunrays moon;
    [SerializeField] Wind wind;

    [SerializeField] GameObject winImage;
    [SerializeField] GameObject loseImage;


    public enum GameState
    {
        MainMenu,
        Preparation,
        Tutorial,
        Playing,
        Paused
    }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public event Action<GameState> OnStateChanged;

    [Header("Level Config")]
    public LevelsData levelsData; // Подключаем ScriptableObject с уровнями
    private LevelConfig currentLevelConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartMainMenu();
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing)
        {
            AudioManager.Instance.StopMusic("rain");
            AudioManager.Instance.StopMusic("snow");
            AudioManager.Instance.StopMusic("wind");
        }

        switch (CurrentState)
        {
            case GameManager.GameState.MainMenu:
                MouseClickSound();
                break;

            case GameManager.GameState.Preparation:
                MouseClickSound();
                break;

            case GameManager.GameState.Tutorial:
                MouseClickSound();
                break;

            case GameManager.GameState.Playing:
                Playing();
                break;

            case GameManager.GameState.Paused:
                MouseClickSound();
                break;
        }
    }

    void MouseClickSound()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AudioManager.Instance.PlayEffect("click");
        }
    }

    void Playing()
    {

    }
    // --------------------------------------------------------------------
    // GAME STATE MANAGEMENT
    // --------------------------------------------------------------------

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log("Game state changed to: " + newState);
    }

    public void StartMainMenu()
    {
        Time.timeScale = 0f;
        AudioManager.Instance.PlayMusic("main"); // Музыка меню
        Debug.Log("Entering MAIN MENU");
        SetState(GameState.MainMenu);
    }

    public void StartTutorial() => SetState(GameState.Tutorial);

    public void PauseGame()
    {
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing) PauseGame();
        else if (CurrentState == GameState.Paused) ResumeGame();
    }

    // --------------------------------------------------------------------
    // LEVEL INITIALIZATION
    // --------------------------------------------------------------------

    public void StartLevel(int levelIndex)
    {
        if (levelsData == null || levelIndex < 0 || levelIndex >= levelsData.levels.Length)
        {
            Debug.LogWarning("Invalid level index or LevelsData not set!");
            return;
        }
        currentLevelConfig = levelsData.levels[levelIndex];

        // Подготовка сцены и игрока
        Time.timeScale = 1f;

        // --------------------------------------------------------------------
        // LEVEL-SPECIFIC LOGIC
        // --------------------------------------------------------------------

        flower.temperature = currentLevelConfig.defFlowerTemp;
        flower.water = currentLevelConfig.defFlowerWater;
        flower.score = currentLevelConfig.score;

        wind.repulsion = currentLevelConfig.windRepulcion;
        wind.windDuration = currentLevelConfig.windDuration;
        wind.minDurationWithNoWind = currentLevelConfig.minDurNoWind;
        wind.maxDurationWithNoWind = currentLevelConfig.maxDurNoWind;

        rain.waterIncreasePoints = currentLevelConfig.rainWaterPoints;

        snowfall.waterIncreasePoints = currentLevelConfig.snopwWaterPoints;
        snowfall.temperatureDecreasePoints = currentLevelConfig.snowTempPoints;
        weatherSwitcher.snowfallDuration = currentLevelConfig.snowDuration;
        weatherSwitcher.maxDurationWithNoSnow = currentLevelConfig.maxDurNoSnow;
        weatherSwitcher.minDurationWithNoSnow = currentLevelConfig.minDurNoSnow;

        //sun.SetRaysCount(currentLevelConfig.sunRaysCount);
        sun.temperatureIncreasePoints = currentLevelConfig.sunRayTempPoints;

        //moon.SetRaysCount(currentLevelConfig.moonRaysCount);
        moon.temperatureIncreasePoints = currentLevelConfig.moonRayTempPoints;

        switch (levelIndex)
        {
            case 0:
                // Логика для первого уровня
                Debug.Log("Level 0: Tutorial or easy start");

                windHolder.SetActive(false);
                weatherSwitcher.StopSnow();
                flower.ResetLevel();
                break;

            case 1:
                // Логика второго уровня
                Debug.Log("Level 1: Medium difficulty");
                weatherSwitcher.StartSnow();
                break;

            case 2:
                // Логика третьего уровня
                Debug.Log("Level 2: Harder level");
                windHolder.SetActive(true);
                break;

            case 3:
                // Логика четвертого уровня
                Debug.Log("Level 3: Boss level");
                break;

            case 4:
                // Логика пятого уровня
                Debug.Log("Level 4: Final challenge");
                break;

            default:
                Debug.LogWarning("No specific logic for this level, using default setup");
                break;
        }
    }
    public void StartGame()
    {
        // Запускаем первый уровень
        StartLevel(0);

        AudioManager.Instance.StopMusic("main");
        AudioManager.Instance.PlayMusic("game");

        AudioManager.Instance.PlayMusic("rain");

        // Сразу ставим игру в Playing
        SetState(GameState.Playing);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");

#if UNITY_EDITOR
        // Останавливаем PlayMode в редакторе
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // Закрываем приложение в билде
    Application.Quit();
#endif
    }

    // --------------------------------------------------------------------
    // GAME FLOW EVENTS
    // --------------------------------------------------------------------

    public void WinGame() => StartPreparation("win");
    public void EndGame() => StartPreparation("lose");

    private void StartPreparation(string reason)
    {
        Time.timeScale = 0f;

        AudioManager.Instance.StopMusic("game");
        AudioManager.Instance.PlayMusic("main");

        switch (reason)
        {
            case "win":
                AudioManager.Instance.PlayEffect("win");
                winImage.SetActive(true);
                loseImage.SetActive(false);
                Debug.Log("Player WIN logic here");
                break;

            case "lose":
                AudioManager.Instance.PlayEffect("lose");
                winImage.SetActive(false);
                loseImage.SetActive(true);
                Debug.Log("Player LOSE logic here");
                break;

            default: Debug.Log("Preparation start"); break;
        }

        SetState(GameState.Preparation);
    }

    // --------------------------------------------------------------------
    // LEVEL DATA ACCESS
    // --------------------------------------------------------------------

    public LevelConfig GetCurrentLevelConfig() => currentLevelConfig;
}
