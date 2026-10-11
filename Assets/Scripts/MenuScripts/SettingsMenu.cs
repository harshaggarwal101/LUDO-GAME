using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Slider qualitySlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer  audioMixer;

    private void Start()
    {
        
    }

    private void refreshSettings()
    {
        qualitySlider.value=Settings.qualityLevel;
        volumeSlider.value=Settings.volume;
        ApplySettings();
    }

    public void ApplySettings()
    {
        Settings.qualityLevel = (int)qualitySlider.value;
        Settings.volume = volumeSlider.value;
        QualitySettings.SetQualityLevel(Settings.qualityLevel);
        audioMixer.SetFloat("Master",Mathf.Log10(Settings.volume)*20);
    }
}
