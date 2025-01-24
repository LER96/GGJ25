using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance => _instance;

    [SerializeField] private List<PlayerHanlder> _players;
    [SerializeField] private CinemachineTargetGroup _targetGroup;


    [SerializeField] private int _targetPlayerAmount = 2;

    [Header("Round Rules")]
    [SerializeField] float _roundLength = 180f;

    float _currentTime = 0f;
    bool _gameStart = false;
    bool _roundStart = false;
    bool _roundTimerEnded = false;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private void Update()
    {
        if (!_gameStart)
            return;

        if (!_roundStart)
            return;

        if (_roundTimerEnded)
        {
            //something, end game or something
            print("round timer ended");
        }
        else
        {
            _currentTime += Time.deltaTime;
        }
        CheckRoundTimerEnded();

    }

    public void AddToCameraTargetGroup(Transform targetTransform)
    {
        _targetGroup.AddMember(targetTransform, 1, 1);
    }

    public void RemoveFromCameraTargetGroup(Transform targetTransform)
    {
        _targetGroup.RemoveMember(targetTransform);
    }

    public void AddPlayer(PlayerHanlder player)
    {
        _players.Add(player);
        _targetGroup.AddMember(player.transform, 1, 1);
        if (AllPlayersConnected())
        {
            StartGame();
        }
    }

    private bool AllPlayersConnected()
    {
        return _players.Count == _targetPlayerAmount;
    }

    public void StartGame()
    {
        _gameStart = true;
        UIManager.Instance.StartGameCountdownCoroutine();
    }

    public void StartRound()
    {
        _roundStart = true;
    }

    public void EndRound()
    {
        _roundStart = false;
        //check player HP
        //if player hp = 0 then win screen
        //if player hp > 0 
        //reset round.
    }

    public void ResetRouund()
    {
        //place players in start location
        //remove all weapons
        
    }

    public void CheckRoundTimerEnded()
    {
        if(_currentTime > _roundLength)
        {
            _roundTimerEnded = true;
        }
    }

    public void ResetRoundStats()
    {
        _currentTime = 0f;
        _gameStart = false;
        _roundStart = false;
        _roundTimerEnded = false;
    }
}
