using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    public Transform firstWinnerPlaceholder;
    public Transform secondWinnerPlaceholder;
    public Transform thirdWinnerPlaceholder;
    public Transform fourthWinnerPlaceholder;

    public Camera resultCamera;

    public static ResultScreen Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void HideCamera()
    {
        resultCamera.gameObject.SetActive(false);
    }

    public void Init(string firstWinnerId, string secondWinnerId, string thirdWinnerId, string fourthWinnerId)
    {
        resultCamera.gameObject.SetActive(true);
        BombTimer.Instance.gameObject.SetActive(false);
        Transform firstWinner = GetCharacter(firstWinnerId).transform;
        Transform secondWinner = GetCharacter(secondWinnerId).transform;
        Transform thirdWinner = GetCharacter(thirdWinnerId).transform;
        Transform fourthWinner = GetCharacter(fourthWinnerId).transform;

        firstWinner.SetParent(firstWinnerPlaceholder, false);
        secondWinner.SetParent(secondWinnerPlaceholder, false);
        thirdWinner.SetParent(thirdWinnerPlaceholder, false);
        fourthWinner.SetParent(fourthWinnerPlaceholder, false);
        firstWinner.localPosition = secondWinner.localPosition = thirdWinner.localPosition = fourthWinner.localPosition = Vector3.zero;
        firstWinner.localScale = secondWinner.localScale = thirdWinner.localScale = fourthWinner.localScale = Vector3.one;
        firstWinner.localEulerAngles = secondWinner.localEulerAngles = thirdWinner.localEulerAngles = fourthWinner.localEulerAngles = Vector3.zero;
    }

    public void Init(string[] winnerIds)
    {
        resultCamera.gameObject.SetActive(true);
        BombTimer.Instance.gameObject.SetActive(false);
        Transform[] placeHolders = new Transform[] { firstWinnerPlaceholder, secondWinnerPlaceholder, thirdWinnerPlaceholder, fourthWinnerPlaceholder };

        for (int i = 0; i < winnerIds.Length; i++)
        {
            StartCoroutine(WaitToMove(winnerIds[i], placeHolders[i]));
        }
    }

    private IEnumerator WaitToMove(string avatarId, Transform placeHolder)
    {

        Transform winner = GetCharacter(avatarId).transform;
        winner.SetParent(placeHolder, false);
        winner.GetComponent<CorgiController>().enabled = false;
        winner.GetComponent<Character>().enabled = false;
        winner.GetComponent<CustomCharacterHorizontalMovement>().enabled = false;
        winner.GetComponent<CharacterLevelBounds>().enabled = false;
        yield return null;
        winner.localPosition = Vector3.zero;
        winner.localScale = Vector3.one;
        winner.localEulerAngles = new Vector3(0, 90, 0);
    }

    public GameObject GetCharacter(string avatarId)
    {
        GameObject go = CustomLevelManager.Instance.PlayerPrefabs.Where(x => x.GetComponent<AvatarId>().id == avatarId).Select(x => x.gameObject).FirstOrDefault();
        return Instantiate(go);
    }
}
