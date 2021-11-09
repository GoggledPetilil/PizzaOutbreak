using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("GameManager Components")]
    public static GameManager instance;
    public bool m_GameStarted;    // To indicate that the game has started, skipping the intro.
    public bool m_PlayerHasDied;  // The player has died.
    public int m_SettingsChangedLevel; // How many settings have been changed.
    public Stage m_StageData;
    [SerializeField] private AudioMixer m_AudioMixer;

    [Header("Audio Components")]
    [SerializeField] private GameObject m_OneShotAudio;
    [SerializeField] private AudioSource m_Audio;
    [SerializeField] private AudioClip m_ConfirmSFX;

    [Header("Settings Saved Data")]
    public float m_MasterVolume;
    public float m_MusicVolume;
    public float m_SoundVolume;
    public float m_VoiceVolume;
    public int m_QualityLevel;
    public int m_ResolutionWidth;
    public int m_ResolutionHeight;
    public bool m_FullScreen;

    [Header("Records Data")]
    public int m_RocketFired;

    void Awake()
    {
        if(GameManager.instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            LoadData();
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public void LoadLevel(int sceneID)
    {
        Debug.Log("Loading Level #" + sceneID);
        SceneManager.LoadScene(sceneID);
    }

    public AsyncOperation LoadLevelAsync(int sceneID)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);
        return asyncLoad;
    }

    public void PlayOneShot(AudioClip clip)
    {
        GameObject go = Instantiate(m_OneShotAudio) as GameObject;
        go.GetComponent<AudioSource>().clip = clip;
    }

    public void PlayConfirmSFX()
    {
        m_Audio.clip = m_ConfirmSFX;
        m_Audio.Play();
    }

    public void SaveData()
    {
        if(m_SettingsChangedLevel < 0) m_SettingsChangedLevel = 0;
        if(m_SettingsChangedLevel <= 0) return;

        PlayerPrefs.SetFloat("masterVolume", m_MasterVolume);
        PlayerPrefs.SetFloat("musicVolume", m_MusicVolume);
        PlayerPrefs.SetFloat("soundVolume", m_SoundVolume);
        PlayerPrefs.SetFloat("voiceVolume", m_VoiceVolume);
        PlayerPrefs.SetInt("qualityLevel", m_QualityLevel);
        PlayerPrefs.SetInt("resolutionWidth", m_ResolutionWidth);
        PlayerPrefs.SetInt("resolutionHeight", m_ResolutionHeight);
        PlayerPrefs.SetInt("fullScreen", m_FullScreen.GetHashCode());

        m_SettingsChangedLevel = 0;
    }

    public void LoadData()
    {
        m_MasterVolume = PlayerPrefs.GetFloat("masterVolume", 0f);
        m_MusicVolume = PlayerPrefs.GetFloat("musicVolume", 0f);
        m_SoundVolume = PlayerPrefs.GetFloat("soundVolume", 0f);
        m_VoiceVolume = PlayerPrefs.GetFloat("voiceVolume", 0f);
        m_QualityLevel = PlayerPrefs.GetInt("qualityLevel", QualitySettings.GetQualityLevel());
        m_ResolutionWidth = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        m_ResolutionHeight = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        m_FullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("fullScreen", Screen.fullScreen.GetHashCode()));

        m_AudioMixer.SetFloat("masterVolume", m_MasterVolume);
        m_AudioMixer.SetFloat("musicVolume", m_MusicVolume);
        m_AudioMixer.SetFloat("soundVolume", m_SoundVolume);
        m_AudioMixer.SetFloat("voiceVolume", m_VoiceVolume);
        QualitySettings.SetQualityLevel(m_QualityLevel, false);
        Screen.SetResolution(m_ResolutionWidth, m_ResolutionHeight, m_FullScreen);

        m_SettingsChangedLevel = 1;
        SaveData();
    }
}
