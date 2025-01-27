using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WVDTutorialDetails
{
    public string TutorialInformation;
    public bool BeenPlayedBefore;


    public WVDTutorialDetails(string tip, bool played)
    {
        TutorialInformation = tip;
        BeenPlayedBefore = played;
    }
}
