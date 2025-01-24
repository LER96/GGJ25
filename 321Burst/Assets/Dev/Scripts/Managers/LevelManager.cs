using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPos
    {
        public Transform _spot;
        public bool _isoccupied;
    }
    private static LevelManager _instance;
    public static event Action OnDeath;
    public static LevelManager Instance => _instance;

    [SerializeField] private List<PlayerHanlder> _players;
    [SerializeField] private List<SpawnPos> _startPos;
    [SerializeField] private List<Transform> _weaponsSpots;
    [SerializeField] private List<Weapon> _weapons;

    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private int _targetPlayerAmount = 2;

    [Header("Round Rules")]
    [SerializeField] float _roundLength = 180f;

    private float _currentTime = 0f;
    private bool _gameStart = false;
    private bool _roundStart = false;
    private bool _roundTimerEnded = false;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        OnDeath += EndRound;
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
        SetSpot(player);
        _targetGroup.AddMember(player.transform, 1, 1);
        if (AllPlayersConnected())
        {
            StartGame();
        }
    }

    void SetSpot(PlayerHanlder player)
    {
        for (int i = 0; i < _startPos.Count; i++)
        {
            if (_startPos[i]._isoccupied == false)
            {
                player.transform.position = _startPos[i]._spot.position;
                _startPos[i]._isoccupied = true;
            }
        }
    }

    private void ResetSpot()
    {
        for (int i = 0; i < _startPos.Count; i++)
        {
            _startPos[i]._isoccupied = false;
            _startPos[i]._spot = null;
        }
    }

    private bool AllPlayersConnected()
    {
        return _players.Count == _targetPlayerAmount;
    }

    public void StartGame()
    {
        _gameStart = true;
        SpawnWeapons();
        UIManager.Instance.StartGameCountdownCoroutine();
    }

    void SpawnWeapons()
    {
        for (int i = 0; i < _weapons.Count; i++)
        {
            Weapon weapon= Instantiate(_weapons[i], _weaponsSpots[i]);
            StartCoroutine(Camera(weapon));
        }
    }

    IEnumerator Camera(Weapon weapon)
    {
        AddToCameraTargetGroup(weapon.transform);
        yield return new WaitForSeconds(3);
        RemoveFromCameraTargetGroup(weapon.transform);
    }

    public void StartRound()
    {
        _roundStart = true;
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].StartRound();
        }
    }

    public void EndRound()
    {
        _roundStart = false;
        for(int i = 0; i < _players.Count; i++)
        {
            if (_players[i].HP == 0)
            {
                EndGame(i+1);
                return;
            }
        }

        ResetRound();
        //reset round.
    }

    public void EndGame(int winningPlayer)
    {
        //turn on winner UI
        UIManager.Instance.EnableWinScreen(winningPlayer);
    }

    public void ResetRound()
    {
        ResetSpot();
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
