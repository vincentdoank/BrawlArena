using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Character character = collision.GetComponent<Character>();
            character.RespawnAt(spawnPoint, Character.FacingDirections.Right);
            character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
            character.GetComponent<CustomCharacterHorizontalMovement>().ResetSpeed();
            character.Reset();
            character.ResetInput();

            foreach (CharacterAbility ability in character.GetComponents<CharacterAbility>())
            {
                ability.PermitAbility(false);
                ability.ResetAbility();
            }

            StartCoroutine(WaitForFreeze(character));
        }
    }

    private IEnumerator WaitForFreeze(Character character)
    {
        yield return new WaitForSeconds(2f);
        foreach (CharacterAbility ability in character.GetComponents<CharacterAbility>())
        {
            ability.PermitAbility(true);
            ability.ResetAbility();
        }
        character.UnFreeze();
    }
}
