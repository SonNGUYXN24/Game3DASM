using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource menuAudioSource; // Âm thanh của MainMenu

    [Header("UI Sliders")]
    [SerializeField] private Slider menuVolumeSlider; // Slider cho MainMenu
    [SerializeField] private Slider inGameVolumeSlider; // Slider cho inGame (điều chỉnh trước khi vào scene inGame)

    private const string MenuVolumeKey = "MenuVolume";
    private const string InGameVolumeKey = "InGameVolume";

    private void Start()
    {
        // Khởi tạo giá trị Slider dựa trên dữ liệu đã lưu
        float menuVolume = PlayerPrefs.GetFloat(MenuVolumeKey, 1.0f); // Giá trị mặc định là 1.0
        float inGameVolume = PlayerPrefs.GetFloat(InGameVolumeKey, 1.0f);

        menuVolumeSlider.value = menuVolume;
        inGameVolumeSlider.value = inGameVolume;

        // Gán sự kiện cho các slider
        menuVolumeSlider.onValueChanged.AddListener(SetMenuVolume);
        inGameVolumeSlider.onValueChanged.AddListener(SetInGameVolume);

        // Áp dụng giá trị âm lượng cho AudioSource trong MainMenu
        menuAudioSource.volume = menuVolume;
    }

    public void SetMenuVolume(float volume)
    {
        // Điều chỉnh âm lượng cho MainMenu
        menuAudioSource.volume = volume;

        // Lưu giá trị vào PlayerPrefs
        PlayerPrefs.SetFloat(MenuVolumeKey, volume);
    }

    public void SetInGameVolume(float volume)
    {
        // Lưu giá trị cho âm thanh inGame
        PlayerPrefs.SetFloat(InGameVolumeKey, volume);
    }
}
