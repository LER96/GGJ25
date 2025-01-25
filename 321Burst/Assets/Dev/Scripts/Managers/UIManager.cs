using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    static UIManager _instance;
    public static UIManager Instance =>_instance;

    [Header("Gameplay")]
    [SerializeField] private RectTransform _gameplayUI;

    [Header("Player Connect UI")]
    [SerializeField] private RectTransform _playerConnectUI;
    [SerializeField] private TextMeshProUGUI _player1JoinedText;
    [SerializeField] private TextMeshProUGUI _player2JoinedText;


    [Header("Start Round Countdown")]
    [SerializeField] private RectTransform _startCountdownUI;
    [SerializeField] private TextMeshProUGUI _countdownText;

    [Header("Win Screen")]
    [SerializeField] private RectTransform _winScreenUI;
    [SerializeField] private TextMeshProUGUI _winPlayerText;

    private void Awake()
    {
        _instance = this;
    }

    public void EnableWinScreen(int player)
    {
        switch (player)
        {
            case 1:
                _winPlayerText.text = "Player 1";
                break;
            case 2:
                _winPlayerText.text = "Player 2";
                break;
            default:
                _winPlayerText.text = "Someone Probably";
                break;
        }
        _winScreenUI.gameObject.SetActive(true);
        GoToMenuCoroutine();
    }

    public void PlayerJoined(int playerNum)
    {
        if(playerNum == 1)
        {
            _player1JoinedText.text = "Joined";
        }
        else if(playerNum == 2)
        {
            _player2JoinedText.text = "Joined";
        }
        else
        {
            _player1JoinedText.text = "Something Went Wrong";
            _player2JoinedText.text = "Something Went Wrong";
        }
    }

    void GoToMenuCoroutine()
    {
        StartCoroutine(MenuCoroutine());
    }

    IEnumerator MenuCoroutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
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
        _startCountdownUI.gameObject.SetActive(false);
        LevelManager.Instance.StartRound();
    }
}
