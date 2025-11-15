using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Sliders")]
    public Flower flower;          // Ссылка на скрипт роста
    public Slider waterSlider;         // Слайдер воды
    public Slider temperatureSlider;   // Слайдер температуры
    public Slider growSlider;          // Слайдер роста
    public Slider energySlider;        // Слайдер энергии (0..5)

    [Header("Texts")]
    public TMP_Text waterText;
    public TMP_Text temperatureText;
    public TMP_Text growText;
    public TMP_Text energyText;
    public TMP_Text levelText;

    void Update()
    {
        if (flower == null) return;

        // Обновляем слайдеры
        waterSlider.value = flower.GetWater();
        temperatureSlider.value = flower.GetTemperature();
        growSlider.value = flower.GetGrow();

        if (energySlider) energySlider.value = flower.GetEnergy();

        // Обновляем текст
        if (waterText) waterText.text = $"Water: {flower.GetWater():0.0}";
        if (temperatureText) temperatureText.text = $"Temp: {flower.GetTemperature():0.0}";
        if (growText) growText.text = $"Grow: {flower.GetGrow():0.0}";
        if (energyText) energyText.text = $"Energy: {flower.GetEnergy():0.0}";
        if (levelText) levelText.text = $"Grow Level: {flower.GetLevel():0.0}";
    }
}
