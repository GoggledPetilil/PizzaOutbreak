using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoList : ListScroll
{
    private Resolution[] resolutions;

    void Awake()
    {
        CreateList();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollThroughList();
    }

    public override void SelectionEvent()
    {
        base.SelectionEvent();

        Resolution resolution = resolutions[m_Value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        if(GameManager.instance.m_ResolutionWidth != resolutions[m_Value].width || GameManager.instance.m_ResolutionHeight != resolutions[m_Value].height)
        {
            if(m_ValueChanged == false)
            {
                m_ValueChanged = true;
                GameManager.instance.m_SettingsChangedLevel++;
            }
        }
        else
        {
            m_ValueChanged = false;
            GameManager.instance.m_SettingsChangedLevel--;
        }
    }

    void CreateList()
    {
        if(resolutions != null) return;

        resolutions = Screen.resolutions; // Gets all possible resolutions for this screen.
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Creating the string and adding it to our Options List.
            string option = resolutions[i].width + " x " + resolutions[i].height;
            m_Options.Add(option);
        }
    }

    public override void LoadSavedSettings()
    {
        CreateList();

        int width = GameManager.instance.m_ResolutionWidth;
        int height = GameManager.instance.m_ResolutionHeight;
        for (int i = 0; i < resolutions.Length; i++)
        {
            // If this string is our current resolution, then we set the m_Value to this resolution's index.
            if(resolutions[i].width == width && resolutions[i].height == height)
            {
                m_Value = i;
                m_Selected = m_Value;
                UpdateDisplay();

                return;
            }
        }
    }

    public override void SaveSettings()
    {
        CreateList();
        Resolution resolution = resolutions[m_Value];

        GameManager.instance.m_ResolutionWidth = resolution.width;
        GameManager.instance.m_ResolutionHeight = resolution.height;
        base.SaveSettings();
    }
}
