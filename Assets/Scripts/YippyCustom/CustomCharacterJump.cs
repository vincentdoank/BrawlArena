using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterJump : CharacterJump
{
	public void SetVerticalMove(float value)
	{
		_verticalInput = value;
	}
}
