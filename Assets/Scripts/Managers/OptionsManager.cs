using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MenuController
{
    [Header("Records Components")]
    [SerializeField] private GameObject m_RecordsTab;
    [SerializeField] private GameObject m_RecordsFirstObj;

    void Awake()
    {
        Init();
    }

    void Start()
    {
        LoadSavedSettings();
    }

    void Update()
    {
        MenuControls();
        CheckSelected();
    }

    public override void EnableMainMenu()
    {
        base.EnableMainMenu();

        m_RecordsTab.SetActive(false);
        m_HeaderText.text = "PIZZERIA";
    }

    public override void EnableSettingsMenu()
    {
        base.EnableSettingsMenu();

        m_RecordsTab.SetActive(false);
    }

    public void EnableRecordsMenu()
    {
        m_RecordsTab.SetActive(true);
        m_MainTab.SetActive(false);
        m_SettingsTab.SetActive(false);

        m_EventSystem.SetSelectedGameObject(m_RecordsFirstObj);

        m_HeaderText.text = "RECORDS";
        m_TitleAnimator.SetTrigger("Change");
        GameManager.instance.PlayConfirmSFX();
    }
}
