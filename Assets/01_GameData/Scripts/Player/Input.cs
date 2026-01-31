using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Input
{
    private readonly struct InputBinding
    {
        public readonly InputAction Act;
        public readonly Action<InputAction.CallbackContext> Handler;

        public InputBinding(InputAction action, Action<InputAction.CallbackContext> handler)
        {
            Act = action;
            Handler = handler;
        }
    }

    // ---------------------------- Fields
    private static readonly PlayerAction _act = new();
    private static readonly PlayerAction.PlayerActions _playerAct = _act.Player;

    public static readonly ReactiveProperty<Vector3> MoveDir = new();
    public static readonly ReactiveProperty<InputActionPhase> OnJump = new();
    public static readonly ReactiveProperty<InputActionPhase> OnFire = new();

    private static readonly List<InputBinding> Bindings = new()
    {
        new InputBinding(
            _playerAct.Move,
            ctx => MoveDir.Value = ctx.ReadValue<Vector2>()
        ),
        new InputBinding(
            _playerAct.Jump,
            ctx => OnJump.Value = ctx.phase
        ),
        new InputBinding(
            _playerAct.Fire,
            ctx => OnFire.Value = ctx.phase
        ),
    };


    // ---------------------------- Public Methods
    public static void SetEnable()
    {
        _act.Enable();
        foreach (var b in Bindings)
        {
            b.Act.started += b.Handler;
            b.Act.performed += b.Handler;
            b.Act.canceled += b.Handler;
        }
    }

    public static void SetDisable()
    {
        foreach (var b in Bindings)
        {
            b.Act.started -= b.Handler;
            b.Act.performed -= b.Handler;
            b.Act.canceled -= b.Handler;
        }
        _act.Disable();
    }
}