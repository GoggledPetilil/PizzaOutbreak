using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityList : ListScroll
{
    void Start()
    {
        m_Selected = m_Value;
        UpdateDisplay();
    }

    void Update()
    {
        ScrollThroughList();
    }

    public override void SelectionEvent()
    {
        base.SelectionEvent();

        QualitySettings.SetQualityLevel(m_Value);
    }
}
