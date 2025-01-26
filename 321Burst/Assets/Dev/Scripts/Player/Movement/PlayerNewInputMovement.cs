using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewInputMovement : Movement
{

    void OnMove(InputValue input)
    {
        if (_moveDelay)
        {
            _movementInput = input.Get<Vector2>();
            _runFeedBack.PlayFeedbacks();
            if (_movementInput.x < 0)
                transform.localScale = new Vector3(-_scale.x, _scale.y, _scale.z);
            else if (_movementInput.x > 0)
                transform.localScale = new Vector3(_scale.x, _scale.y, _scale.z);
            else
                _idleFeedBack.PlayFeedbacks();
        }
    }

    void OnJump(InputValue input)
    {
        if (input.isPressed && _jump == false)
        {
            Jump();
        }

        _jump = input.isPressed;
    }

}
