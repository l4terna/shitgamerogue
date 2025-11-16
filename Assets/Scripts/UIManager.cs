using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public Flower flower;

    [Header("Sliders")]
    public Slider waterSlider;
    public Slider temperatureSlider;
    public Slider growSlider;
    public Slider energySlider;

    [Header("Texts")]
    public TMP_Text waterText;
    public TMP_Text temperatureText;
    public TMP_Text growText;
    public TMP_Text energyText;
    public TMP_Text levelText;

    [Header("UI Panels by State")]
    public GameObject mainMenuUI;
    public GameObject tutorialUI;
    public GameObject gameplayUI;
    public GameObject pauseUI;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void Start()
    {
        // При старте сразу выставляем UI по текущему состоянию
        OnGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void Update()
    {
        if (flower == null) return;

        // Обновление значений при игре
        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing ||
            GameManager.Instance.CurrentState == GameManager.GameState.Tutorial)
        {
            UpdateUIValues();
        }
    }

    private void UpdateUIValues()
    {
        // Слайдеры
        waterSlider.value = flower.GetWater();
        temperatureSlider.value = flower.GetTemperature();
        growSlider.value = flower.GetGrow();
        if (energySlider) energySlider.value = flower.GetEnergy();

        // Тексты
        if (waterText) waterText.text = $"Water: {flower.GetWater():0.0}";
        if (temperatureText) temperatureText.text = $"Temp: {flower.GetTemperature():0.0}";
        if (growText) growText.text = $"Grow: {flower.GetGrow():0.0}";
        if (energyText) energyText.text = $"Energy: {flower.GetEnergy():0.0}";
        if (levelText) levelText.text = $"Grow Level: {flower.GetLevel():0}";
    }

    // ---------------------------
    // Реакция UI на смену состояния
    // ---------------------------
    private void OnGameStateChanged(GameManager.GameState state)
    {
        // Отключаем всё
        if (mainMenuUI) mainMenuUI.SetActive(false);
        if (tutorialUI) tutorialUI.SetActive(false);
        if (gameplayUI) gameplayUI.SetActive(false);
        if (pauseUI) pauseUI.SetActive(false);

        // Включаем нужное
        switch (state)
        {
            case GameManager.GameState.MainMenu:
                if (mainMenuUI) mainMenuUI.SetActive(true);
                break;

            case GameManager.GameState.Tutorial:
                if (tutorialUI) tutorialUI.SetActive(true);
                break;

            case GameManager.GameState.Playing:
                if (gameplayUI) gameplayUI.SetActive(true);
                break;

            case GameManager.GameState.Paused:
                if (pauseUI) pauseUI.SetActive(true);
                break;
        }
    }
}
