using Cinemachine;
using MoreMountains.Feedbacks;
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
    [SerializeField] private GameObject _hpbars;
    [SerializeField] private List<GameObject> _player1Bar = new List<GameObject>();
    [SerializeField] private List<GameObject> _player2Bar = new List<GameObject>();
    [SerializeField] private List<SpawnPos> _playerStartPositions;
    [SerializeField] private List<Transform> _weaponsSpots;
    private Dictionary<Transform, bool> _spawnPointAvailability= new Dictionary<Transform, bool>();
    [SerializeField] private List<Weapon> _weapons;
    [SerializeField] private List<Weapon> _spawnedWeapons;

    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private int _targetPlayerAmount = 2;

    [Header("Round Rules")]
    [SerializeField] float _roundLength = 180f;
    [Header("Weapon Rules")]
    [SerializeField] int _amountOfStartingWeapons = 2;
    [SerializeField] float _timePerWeaponSpawn = 10f;
    [SerializeField] int _maxWeaponsOnBoard = 4;

    private float _currentTime = 0f;
    private float _currentWeaponTimer = 0f;
    private bool _gameStart = false;
    private bool _roundStart = false;
    private bool _roundTimerEnded = false;
    private int _currentWeaponIndex = 0;

    [SerializeField] MMF_Player _mainSound;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        OnDeath += EndRound;
    }

    private void Start()
    {
        _hpbars.SetActive(false);
        InitializeWeaponSpawnPoints();
        _mainSound.PlayFeedbacks();
        _currentTime = _roundLength;
        UIManager.Instance.UpdateCountdownTimer(_currentTime);
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
            _currentTime -= Time.deltaTime;
            UIManager.Instance.UpdateCountdownTimer(_currentTime);
        }
        CheckRoundTimerEnded();

        HandleWeaponSpawning();
    }

    private void HandleWeaponSpawning()
    {
        if (_currentWeaponTimer < _timePerWeaponSpawn)
        {
            _currentWeaponTimer += Time.deltaTime;
        }
        else
        {
            _currentWeaponTimer = 0f;

            //check if should spawn weapon
            int count = 0;
            foreach (var sp in _spawnPointAvailability)
            {
                if (!sp.Value)
                {
                    count++;
                }
            }
            if (count < _maxWeaponsOnBoard)
                SpawnWeapons(1);
        }
    }

    private void InitializeWeaponSpawnPoints()
    {
        _spawnPointAvailability = new Dictionary<Transform, bool>();
        foreach (var spot in _weaponsSpots)
        {
            _spawnPointAvailability[spot] = true; // All spawn points are initially available
        }
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
        player.SetBubbleIndex(_players.Count - 1);
        UIManager.Instance.PlayerJoined(_players.Count);
        SetSpot(player);
        _targetGroup.AddMember(player.transform, 1, 1);
        if (AllPlayersConnected())
        {
            StartGame();
        }
    }

    void SetSpot(PlayerHanlder player)
    {
        for (int i = 0; i < _playerStartPositions.Count; i++)
        {
            if (_playerStartPositions[i]._isoccupied == false)
            {
                player.transform.position = _playerStartPositions[i]._spot.position;
                _playerStartPositions[i]._isoccupied = true;
                return;
            }
        }
    }

    private void ResetSpot()
    {
        for (int i = 0; i < _playerStartPositions.Count; i++)
        {
            _playerStartPositions[i]._isoccupied = false;
        }
    }

    private bool AllPlayersConnected()
    {
        return _players.Count == _targetPlayerAmount;
    }

    public void StartGame()
    {
        _hpbars.SetActive(true);
        foreach (var player in _players)
        {
            player.dead = false;
            player.PlayerMovement.StopMovement();
            player.PlayerMovement.PlayerAnimator.Play("PlayerIdle");
        }

        _gameStart = true;
        _currentWeaponIndex = UnityEngine.Random.Range(0, _weapons.Count - 1);
        SpawnWeapons(_amountOfStartingWeapons);
        UIManager.Instance.StartGameCountdownCoroutine();
    }

    void SpawnWeapons(int amountOfWeaponsToSpawn)
    {
        // for each weapon, spawn in available spawn point, make spawn point used, set weapon spawn point
        CameraManager.Instance.Shake(0.3f);
        for (int i = 0; i < amountOfWeaponsToSpawn; i++)
        {
            Transform spawnPoint = GetRandomAvailableWeaponSpawnPoint();
            Weapon weapon = Instantiate(_weapons[_currentWeaponIndex], spawnPoint);
            weapon.SetSpawnPoint(spawnPoint);
            _spawnedWeapons.Add(weapon);
            StartCoroutine(Camera(weapon));

            if (_currentWeaponIndex + 1 < _weapons.Count)
                _currentWeaponIndex++;
            else
                _currentWeaponIndex = 0;
        }
    }

    public Transform GetRandomAvailableWeaponSpawnPoint()
    {
        List<Transform> availableSpots = new List<Transform>();

        foreach (var kvp in _spawnPointAvailability)
        {
            if (kvp.Value) // Check if the spawn point is available
            {
                availableSpots.Add(kvp.Key);
            }
        }

        if (availableSpots.Count == 0)
        {
            Debug.LogWarning("No available spawn points!");
            return null;
        }

        // Pick a random available spot
        Transform randomSpot = availableSpots[UnityEngine.Random.Range(0, availableSpots.Count)];
        MarkSpawnPointAsUsed(randomSpot);
        return randomSpot;
    }

    public void MarkSpawnPointAsUsed(Transform spawnPoint)
    {
        if (_spawnPointAvailability.ContainsKey(spawnPoint))
        {
            _spawnPointAvailability[spawnPoint] = false;
        }
        else
        {
            Debug.LogWarning("Trying to mark an invalid spawn point as used.");
        }
    }

    public void MarkSpawnPointAsAvailable(Transform spawnPoint)
    {
        if (_spawnPointAvailability.ContainsKey(spawnPoint))
        {
            _spawnPointAvailability[spawnPoint] = true;
            Debug.Log($"{spawnPoint.name} is now available.");
        }
        else
        {
            Debug.LogWarning("Trying to mark an invalid spawn point as available.");
        }
    }

    IEnumerator Camera(Weapon weapon)
    {
        AddToCameraTargetGroup(weapon.transform);
        yield return new WaitForSeconds(3);
        if (weapon != null)
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
        for (int i = 0; i < _players.Count; i++)
        {
            SetBar(_player1Bar, _players[0].HP);
            if (_players.Count > 1)
                SetBar(_player2Bar, _players[1].HP);

            if (_players[i].HP == 0)
            {
                EndGame(2 - i);
                return;
            }
        }

        ResetRound();
        //reset round.
    }

    void SetBar(List<GameObject> bar, int hp)
    {
        for (int i = 0; i < bar.Count; i++)
        {
            if (i < hp)
            {
                bar[i].SetActive(true);
            }
            else
                bar[i].SetActive(false);
        }
    }

    public void EndGame(int winningPlayer)
    {
        //turn on winner UI
        UIManager.Instance.EnableWinScreen(winningPlayer);
    }

    public void ResetRound()
    {
        ResetSpot();
        InitializeWeaponSpawnPoints();
        ResetRoundStats();

        foreach (var weapon in _spawnedWeapons)
        {
            weapon.ForceDropWeapon();
            Destroy(weapon.gameObject);
        }
        _spawnedWeapons.Clear();

        foreach(var player in _players)
        {
            player.WeaponHandler.CurrentWeapon = null;
        }

        StartCoroutine(DelayRoundStart());


    }

    IEnumerator DelayRoundStart()
    {
        yield return new WaitForSeconds(2f);
        foreach (var player in _players)
        {
            SetSpot(player);
            player.RoundReset();
        }
        StartGame();
    }
    public void CheckRoundTimerEnded()
    {
        if (_currentTime <= 0)
        {
            _roundTimerEnded = true;
            //kill players
            KillPlayersAndEndRound();
        }
    }

    void KillPlayersAndEndRound()
    {
        foreach (var player in _players)
        {
            player.KillPlayer();
        }

        EndRound();
    }

    public void ResetRoundStats()
    {
        _currentTime = _roundLength;
        _gameStart = false;
        _roundStart = false;
        _roundTimerEnded = false;
    }
}
