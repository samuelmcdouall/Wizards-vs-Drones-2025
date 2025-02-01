using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDDisplayTutorialEventData : WVDEventData
{
    public WVDTutorialManager.TutorialPart Part;
    public float Delay;

    public WVDDisplayTutorialEventData(WVDTutorialManager.TutorialPart part, float delay)
    {
        Part = part;
        Delay = delay;
    }
}
