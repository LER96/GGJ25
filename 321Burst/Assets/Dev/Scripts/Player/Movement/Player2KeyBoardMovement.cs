using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2KeyBoardMovement : Movement
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
        if (_canMove)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                _movementInput.x = -1;
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
                _movementInput.x = 0;
            if (Input.GetKeyDown(KeyCode.RightArrow))
                _movementInput.x = 1;
            else if (Input.GetKeyUp(KeyCode.RightArrow))
                _movementInput.x = 0;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
                _movementInput.y = 1;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                _movementInput.y = -1;

        }

        //if (Input.GetKeyDown(KeyCode.RightControl))
        //    _playerHandler.WeaponHandler.Attack.Invoke();

        //if (Input.GetKeyDown(KeyCode.RightShift))
        //    _playerHandler.WeaponHandler.Pick.Invoke();

    }
}
