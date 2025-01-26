using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreatePlayers : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab;

    private void Start()
    {
        StartCoroutine(DelaySpawn());
    }

    IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(1);
        PlayerInput player1 = PlayerInput.Instantiate(_playerPrefab, controlScheme: "KeyBoard_WASD", pairWithDevice: Keyboard.current);
        PlayerInput player2 = PlayerInput.Instantiate(_playerPrefab, controlScheme: "KeyBoard_Arrow", pairWithDevice: Keyboard.current);
    }
}
