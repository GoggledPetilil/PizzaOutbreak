using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MenuController
{
    public static PauseManager instance;

    [Header("Pause Components")]
    public bool m_IsPaused;
    public GameObject m_PauseCanvas;
    [SerializeField] private GameObject m_HudCanvas;

    [Header("Audio Components")]
    [SerializeField] private AudioSource m_Audio;
    [SerializeField] private AudioClip m_PauseSound;
    [SerializeField] private AudioClip m_ResumeSound;

    void Awake()
    {
        instance = this;
        m_Audio = GetComponent<AudioSource>();

        Init();
    }

    void Start()
    {
        LoadSavedSettings();
        Resume();
        if(m_PauseCanvas.activeSelf) m_PauseCanvas.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if(m_IsPaused == false) return;

        MenuControls();
        CheckSelected();
    }

    public void Resume()
    {
        if(m_IsPaused == false) return;

        Time.timeScale = 1f;
        m_IsPaused = false;
        m_PauseCanvas.SetActive(false);
        m_HudCanvas.SetActive(true);

        m_Audio.clip = m_ResumeSound;
        m_Audio.Play();
        m_AudioMixer.SetFloat("musicVolume", m_MusicSlider.value);
        GameManager.instance.SaveData();
    }

    public void Pause()
    {
        if(m_IsPaused == true) return;

        Time.timeScale = 0f;
        m_IsPaused = true;
        m_PauseCanvas.SetActive(true);
        m_HudCanvas.SetActive(false);
        EnableMainMenu();

        m_Audio.clip = m_PauseSound;
        m_Audio.Play();
        m_AudioMixer.SetFloat("musicVolume", -80f);

        Init();
    }

    public override void EnableMainMenu()
    {
        base.EnableMainMenu();

        m_HeaderText.text = "Taking a break...";
        m_AudioMixer.SetFloat("musicVolume", -80f);
    }

    public override void ExitLevel()
    {
        Time.timeScale = 1f;
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().FreezeMovement(true);
        player.GetComponent<BoxCollider2D>().enabled = false;

        base.ExitLevel();
    }
}
