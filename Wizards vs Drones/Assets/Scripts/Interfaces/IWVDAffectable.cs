using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWVDAffectable // todo may not be needed if all these are just going in the base class
{
    void ApplyEffects(WVDAttackEffects effects);

    void ApplySlow(float percentage, float time);
}
