using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MenuController
{
    [Header("Menu Components")]
    [SerializeField] private TMP_Text m_HeaderText;

    [Header("Main Menu Components")]
    [SerializeField] private GameObject m_MainContent;
    [SerializeField] private GameObject m_MainFirstObj;

    [Header("Records Components")]
    [SerializeField] private GameObject m_RecordsContent;
    [SerializeField] private GameObject m_RecordsFirstObj;

    [Header("Settings Components")]
    [SerializeField] private GameObject m_SettingsContent;
    [SerializeField] private GameObject m_SettingsFirstObj;
    private RectTransform m_SettingsTransform;
    private List<Selectable> m_SettingsElements = new List<Selectable>();


    void Awake()
    {
        Init();

        m_SettingsTransform = m_SettingsContent.GetComponent<RectTransform>();
        m_SettingsContent.GetComponentsInChildren(m_SettingsElements);

        EnableMainMenu();
    }

    void Update()
    {
        if(Input.GetButtonUp("Cancel") && m_MainContent.activeSelf == false)
        {
            EnableMainMenu();
        }

        if(m_SettingsContent.activeSelf)
        {
            if(m_EventSystem.currentSelectedGameObject != m_SelectedObject && m_EventSystem.currentSelectedGameObject != null)
            {
                int i = m_SettingsElements.IndexOf(m_EventSystem.currentSelectedGameObject.GetComponent<Selectable>());
                if(i < 0)
                {
                    foreach(Selectable parent in m_SettingsElements)
                    {
                        if(m_EventSystem.currentSelectedGameObject.transform.IsChildOf(parent.transform))
                        {
                            i = m_SettingsElements.IndexOf(parent);
                            return;
                        }
                    }
                }

                SettingsPosUpdate(i);
            }
        }

        CheckSelected();
    }

    public void EnableMainMenu()
    {
        m_MainContent.SetActive(true);
        m_RecordsContent.SetActive(false);
        m_SettingsContent.SetActive(false);

        m_EventSystem.SetSelectedGameObject(m_MainFirstObj);

        m_HeaderText.text = "PIZZERIA";
        GameManager.instance.PlayConfirmSFX();
    }

    public void EnableRecordsMenu()
    {
        m_RecordsContent.SetActive(true);
        m_MainContent.SetActive(false);
        m_SettingsContent.SetActive(false);

        m_EventSystem.SetSelectedGameObject(m_RecordsFirstObj);

        m_HeaderText.text = "RECORDS";
        GameManager.instance.PlayConfirmSFX();
    }

    public void EnableSettingsMenu()
    {
        m_SettingsContent.SetActive(true);
        m_MainContent.SetActive(false);
        m_RecordsContent.SetActive(false);

        m_EventSystem.SetSelectedGameObject(m_SettingsFirstObj);

        m_HeaderText.text = "SETTINGS";
        GameManager.instance.PlayConfirmSFX();

        SettingsPosUpdate(0);
    }

    public void SetMasterVolume(float volume)
    {
        m_AudioMixer.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        m_AudioMixer.SetFloat("musicVolume", volume);
    }

    public void SetSoundVolume(float volume)
    {
        m_AudioMixer.SetFloat("soundVolume", volume);
    }

    public void SetVoiceVolume(float volume)
    {
        m_AudioMixer.SetFloat("voiceVolume", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ExitLevel()
    {
        GameManager.instance.LoadLevel(0);
    }

    public void SettingsPosUpdate(int index)
    {
        float maxHeight = m_SettingsTransform.sizeDelta.y / 2; // So, get half.
        float step = maxHeight / (m_SettingsElements.Count - 1);
        float posY = index * step;

        m_SettingsTransform.anchoredPosition = new Vector2(m_SettingsTransform.anchoredPosition.x, posY);
    }
}
