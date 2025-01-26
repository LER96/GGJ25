using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1KeyBoardMovement : Movement
{
    protected override void Start()
    {
        _playerHandler = GetComponent<PlayerHanlder>();
        _scale = transform.localScale;
        _playerHandler.WeaponHandler.AttackEvent += DelayMovement;
    }
    protected override void Update()
    {
        base.Update();
        CheckInput();

    }

    void CheckInput()
    {

        //if (Input.GetKeyDown(KeyCode.A))
        //    _movementInput.x = -1;
        //else if (Input.GetKeyDown(KeyCode.D))
        //    _movementInput.x = 1;
        //else
        //    _movementInput.x = 0;

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Jump();
        //    _movementInput.y = 1;
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //    _movementInput.y = -1;
        //else
        //    _movementInput.y = 0;

        //if (Input.GetKeyDown(KeyCode.Q))
        //    _playerHandler.WeaponHandler.Attack.Invoke();

        //if (Input.GetKeyDown(KeyCode.E))
        //    _playerHandler.WeaponHandler.Pick.Invoke();

    }
}
