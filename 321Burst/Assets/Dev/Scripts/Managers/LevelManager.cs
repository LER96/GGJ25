using Cinemachine;
using MoreMountains.Feedbacks;
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

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public void AddPlayer(PlayerHanlder player)
    {
        _players.Add(player);
        if (AllPlayersConnected())
        {
            //start countdown
        }
    }

    private bool AllPlayersConnected()
    {
        return _players.Count == _targetPlayerAmount;
    }

}
