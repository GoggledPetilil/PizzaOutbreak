using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoList : ListScroll
{
    private Resolution[] resolutions;

    void Awake()
    {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();

        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        m_Options.AddRange(options);
        m_Value = currentResIndex;
        m_Selected = m_Value;

        UpdateDisplay();
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
    }
}
