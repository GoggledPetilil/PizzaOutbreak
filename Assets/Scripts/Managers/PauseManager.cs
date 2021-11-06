using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MenuController
{
    [Header("Pause Components")]
    public static PauseManager instance;
    public GameObject m_PauseCanvas;
    public bool m_IsPaused;

    [Header("Audio Components")]
    [SerializeField] private AudioSource m_Audio;
    [SerializeField] private AudioClip m_PauseSound;
    [SerializeField] private AudioClip m_ResumeSound;

    void Awake()
    {
        instance = this;
        m_Audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        Resume();
    }

    // Update is called once per frame
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

        CheckSelected();
    }

    public void Resume()
    {
        if(m_IsPaused == false) return;

        Time.timeScale = 1f;
        m_IsPaused = false;
        m_PauseCanvas.SetActive(false);

        m_Audio.clip = m_ResumeSound;
        m_Audio.Play();
    }

    public void Pause()
    {
        if(m_IsPaused == true) return;

        Time.timeScale = 0f;
        m_IsPaused = true;
        m_PauseCanvas.SetActive(true);

        m_Audio.clip = m_PauseSound;
        m_Audio.Play();

        Init();
    }

    public void ExitLevel()
    {
        Time.timeScale = 1f;
        GameManager.instance.LoadLevel(0);
    }
}
