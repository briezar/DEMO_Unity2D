using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase MoveBase { get; set; }

    public Move(MoveBase moveBase)
    {
        MoveBase = moveBase;
    }
    public void PlayMoveSFX()
    {
        if (MoveBase.MoveSFX != null) SoundManager.Instance.PlaySFX(MoveBase.MoveSFX);
    }
}
