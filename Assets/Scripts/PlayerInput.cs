using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        playerInputActions = new();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMoveVector()
    {
        Vector2 inputVector = playerInputActions.Player.Walk.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
