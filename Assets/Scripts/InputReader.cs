using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Ctrls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]

public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<Vector2> MoveEvent;
    public event Action<bool> PrimaryFireEvent;
    private Ctrls ctrls;

    public Vector2 aimPos { get; private set; }

    private void OnEnable()
    {
        if( ctrls == null )
        {
            ctrls = new Ctrls();
            ctrls.Player.SetCallbacks( this );
        }

        ctrls.Player.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        else if(context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimPos = context.ReadValue<Vector2>();
    }
}
