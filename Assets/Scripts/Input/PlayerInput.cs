using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput :
ScriptableObject,
InputActions.IGamePlayActions
{
    public event UnityAction<Vector2> onMove = delegate{};
    public event UnityAction onStopMove = delegate{};

    InputActions inputActions;

    void OnEnable() 
    {

        inputActions = new InputActions();
        inputActions.GamePlay.SetCallbacks(this);

    }

    private void OnDisable() 
    {
        DisableAllInputs();
    }

    void SwitchActionMap(InputActionMap actionMap, bool isUIInput)
    {
        inputActions.Disable();
        actionMap.Enable();

        if (isUIInput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else 
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void DisableAllInputs() => inputActions.Disable();

    public void EnableGameplayInput() => SwitchActionMap(inputActions.GamePlay, false);

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) onMove.Invoke(context.ReadValue<Vector2>());

        if (context.canceled) onStopMove.Invoke();
    }
}
