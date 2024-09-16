using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WVDPlayer : WVDEntity
{

    public override void Start()
    {
        base.Start();
        CurrentPlayingAnimation = WVDAnimationStrings.PlayerIdleAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
