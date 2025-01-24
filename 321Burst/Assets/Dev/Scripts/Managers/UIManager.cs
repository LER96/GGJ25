using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    public static UIManager Instance =>_instance;

    [SerializeField] private RectTransform _playerConnectUI;
    [SerializeField] private RectTransform _winScreenUI;
    [SerializeField] private RectTransform _gameplayUI;
    [SerializeField] private RectTransform _startCountdownUI;
    [SerializeField] private TextMeshProUGUI _countdownText;


    private void Awake()
    {
        _instance = this;
    }

    public void EnableWinScreen(int player)
    {
        //turn on UI
        //if player 1 then write 1
        //if player 2 then write 2
    }

    public void StartGameCountdownCoroutine()
    {
        StartCoroutine(StartGameCountdown());
    }

    IEnumerator StartGameCountdown()
    {
        //disable playeconnect ui
        _playerConnectUI.gameObject.SetActive(false);

        //START COUNTDOWN
        _countdownText.text = "3";
        _startCountdownUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(.7f);
        _countdownText.text = "2";
        yield return new WaitForSeconds(.7f);
        _countdownText.text = "1";
        yield return new WaitForSeconds(.7f);
        _countdownText.text = "BURST";
        yield return new WaitForSeconds(.7f);
        _startCountdownUI.gameObject.SetActive(true);
        LevelManager.Instance.StartRound();
    }
}
