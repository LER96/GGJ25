using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Vector2 _movemetInput;

    public Vector2 MovmentInput=> _movemetInput;

    private void Awake()
    {
        Instance = this;
    }

    void OnMove(InputValue input)
    {
        _movemetInput= input.Get<Vector2>();
    }
}
