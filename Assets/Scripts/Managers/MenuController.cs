using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Menu Controller")]
    [SerializeField] protected EventSystem m_EventSystem;
    [SerializeField] protected AudioMixer m_AudioMixer;
    [SerializeField] protected GameObject m_SelectedObject; // The interactable that the game is supposed to have selected.

    [Header("Menu Components")]
    [SerializeField] protected TMP_Text m_HeaderText;
    [SerializeField] protected Animator m_TitleAnimator;
    [SerializeField] protected GameObject m_LoadingScreen;

    [Header("Main Menu Components")]
    [SerializeField] protected GameObject m_MainTab;
    [SerializeField] protected GameObject m_MainFirstObj;

    [Header("Settings Components")]
    [SerializeField] protected GameObject m_SettingsTab;
    [SerializeField] protected GameObject m_SettingsContent;
    [SerializeField] protected GameObject m_SettingsFirstObj;
    protected RectTransform m_SettingsTransform;
    protected List<Selectable> m_SettingsElements = new List<Selectable>();

    [Header("Settings Interactables")]
    [SerializeField] protected Slider m_MasterSlider;
    [SerializeField] protected Slider m_MusicSlider;
    [SerializeField] protected Slider m_SoundSlider;
    [SerializeField] protected Slider m_VoiceSlider;
    [SerializeField] protected QualityList m_QualityList;
    [SerializeField] protected ResoList m_ResoList;
    [SerializeField] protected Toggle m_FullScreenToggle;
    private bool m_ChangedMaster;
    private bool m_ChangedMusic;
    private bool m_ChangedSound;
    private bool m_ChangedVoice;

    protected void Init()
    {
        m_EventSystem.SetSelectedGameObject(null);
        m_SelectedObject = m_EventSystem.firstSelectedGameObject;

        m_SettingsTransform = m_SettingsContent.GetComponent<RectTransform>();
        m_SettingsContent.GetComponentsInChildren(m_SettingsElements);

        m_TitleAnimator.gameObject.SetActive(true);
        m_LoadingScreen.SetActive(false);

        EnableMainMenu();
    }

    protected void CheckSelected()
    {
        if(m_EventSystem.currentSelectedGameObject == null)
        {
            m_EventSystem.SetSelectedGameObject(m_SelectedObject);
        }
        else if(m_EventSystem.currentSelectedGameObject != m_SelectedObject)
        {
            // Because this only activates when the currentSelectedObject != null,
            // m_SelectedObject will never be null.
            m_SelectedObject = m_EventSystem.currentSelectedGameObject;
        }
    }

    protected void MenuControls()
    {
        if(Input.GetButtonUp("Cancel") && m_MainTab.activeSelf == false)
        {
            EnableMainMenu();
            GameManager.instance.PlayCancelSFX();
        }

        if(m_SettingsTab.activeSelf)
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
    }

    public void CursorSound()
    {
        GameManager.instance.PlayCursorSFX();
    }

    public void ConfirmSound()
    {
        GameManager.instance.PlayConfirmSFX();
    }

    protected void LoadSavedSettings()
    {
        m_MasterSlider.value = GameManager.instance.m_MasterVolume;
        m_MusicSlider.value = GameManager.instance.m_MusicVolume;
        m_SoundSlider.value = GameManager.instance.m_SoundVolume;
        m_VoiceSlider.value = GameManager.instance.m_VoiceVolume;
        m_FullScreenToggle.isOn = GameManager.instance.m_FullScreen;
        m_QualityList.LoadSavedSettings();
        m_ResoList.LoadSavedSettings();

        m_ChangedMaster = false;
        m_ChangedMusic = false;
        m_ChangedSound = false;
        m_ChangedVoice = false;
        GameManager.instance.m_SettingsChangedLevel = 0;
    }

    protected void SaveSettings()
    {
        GameManager.instance.m_MasterVolume = m_MasterSlider.value;
        GameManager.instance.m_MusicVolume = m_MusicSlider.value;
        GameManager.instance.m_SoundVolume = m_SoundSlider.value;
        GameManager.instance.m_VoiceVolume = m_VoiceSlider.value;
        GameManager.instance.m_FullScreen = m_FullScreenToggle.isOn;
        m_QualityList.SaveSettings();
        m_ResoList.SaveSettings();

        m_ChangedMaster = false;
        m_ChangedMusic = false;
        m_ChangedSound = false;
        m_ChangedVoice = false;
        GameManager.instance.m_SettingsChangedLevel = 0;
    }

    public virtual void EnableMainMenu()
    {
        m_MainTab.SetActive(true);
        m_SettingsTab.SetActive(false);

        m_EventSystem.SetSelectedGameObject(m_MainFirstObj);

        m_HeaderText.text = "MAIN MENU";
        m_TitleAnimator.SetTrigger("Change");

        if(GameManager.instance.m_SettingsChangedLevel < 1) return;

        SaveSettings();
        GameManager.instance.SaveData();
    }

    public virtual void EnableSettingsMenu()
    {
        m_SettingsTab.SetActive(true);
        m_MainTab.SetActive(false);

        m_EventSystem.SetSelectedGameObject(m_SettingsFirstObj);

        m_HeaderText.text = "SETTINGS";
        m_TitleAnimator.SetTrigger("Change");

        SettingsPosUpdate(0);
    }

    public virtual void ExitLevel()
    {
        GameManager.instance.SaveData();
        StartCoroutine("GoToMap");
    }

    public void SetMasterVolume(float volume)
    {
        Mathf.Round(volume);
        m_AudioMixer.SetFloat("masterVolume", volume);

        if(GameManager.instance.m_MasterVolume != volume && m_ChangedMaster == false)
        {
            GameManager.instance.m_SettingsChangedLevel++;
            m_ChangedMaster = true;
        }
        else if(GameManager.instance.m_MasterVolume == volume)
        {
            GameManager.instance.m_SettingsChangedLevel--;
            m_ChangedMaster = false;
        }
    }

    public void SetMusicVolume(float volume)
    {
        Mathf.Round(volume);
        m_AudioMixer.SetFloat("musicVolume", volume);

        if(GameManager.instance.m_MusicVolume != volume && m_ChangedMusic == false)
        {
            GameManager.instance.m_SettingsChangedLevel++;
            m_ChangedMusic = true;
        }
        else if(GameManager.instance.m_MusicVolume == volume)
        {
            GameManager.instance.m_SettingsChangedLevel--;
            m_ChangedMusic = false;
        }
    }

    public void SetSoundVolume(float volume)
    {
        Mathf.Round(volume);
        m_AudioMixer.SetFloat("soundVolume", volume);

        if(GameManager.instance.m_SoundVolume != volume && m_ChangedSound == false)
        {
            GameManager.instance.m_SettingsChangedLevel++;
            m_ChangedSound = true;
        }
        else if(GameManager.instance.m_SoundVolume == volume)
        {
            GameManager.instance.m_SettingsChangedLevel--;
            m_ChangedSound = false;
        }
    }

    public void SetVoiceVolume(float volume)
    {
        Mathf.Round(volume);
        m_AudioMixer.SetFloat("voiceVolume", volume);

        if(GameManager.instance.m_VoiceVolume != volume && m_ChangedVoice == false)
        {
            GameManager.instance.m_SettingsChangedLevel++;
            m_ChangedVoice = true;
        }
        else if(GameManager.instance.m_VoiceVolume == volume)
        {
            GameManager.instance.m_SettingsChangedLevel--;
            m_ChangedVoice = false;
        }
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        if(GameManager.instance.m_FullScreen != isFullscreen)
        {
            GameManager.instance.m_SettingsChangedLevel++;
        }
        else
        {
            GameManager.instance.m_SettingsChangedLevel--;
        }
    }

    public void SettingsPosUpdate(int index)
    {
        float maxHeight = m_SettingsTransform.sizeDelta.y / 2; // So, get half.
        float step = maxHeight / (m_SettingsElements.Count - 1);
        float posY = index * step;

        m_SettingsTransform.anchoredPosition = new Vector2(m_SettingsTransform.anchoredPosition.x, posY);
    }

    IEnumerator GoToMap()
    {
        m_LoadingScreen.SetActive(true);
        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = GameManager.instance.LoadLevelAsync(0);
        asyncLoad.allowSceneActivation = false;

        while(!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
