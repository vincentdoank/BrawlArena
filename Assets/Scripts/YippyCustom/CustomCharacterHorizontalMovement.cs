using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WTI.NetCode;

public class CustomCharacterHorizontalMovement : CharacterHorizontalMovement
{
	protected override void HandleInput()
	{
		if (!NetworkController.Instance.isServer)
		{
			base.HandleInput();
		}
	}

    public override void ResetInput()
    {
        base.ResetInput();
		_controller.SetHorizontalForce(0f);
    }

	public void ResetSpeed()
	{
		_normalizedHorizontalSpeed = 0f;
	}
}
