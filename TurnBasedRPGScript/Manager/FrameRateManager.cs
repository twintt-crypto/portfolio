using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FrameRateManager : Singleton<FrameRateManager>
{
    public void Initialize()
    {
        SetFrame(120);
        QualitySettings.vSyncCount = -1;
        SetRenderInterval(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRenderInterval(int interval)
    {
        OnDemandRendering.renderFrameInterval = interval;
    }

    public void SetFrame(int frame)
    {
        Application.targetFrameRate = frame;
    }
}
