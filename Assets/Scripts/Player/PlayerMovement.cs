using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]

    [Header("Player Settings")] 
    [SerializeField] private float _speed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private bool _walking;

    private Vector2 _movementDirection;
    
    private int _state;
    private bool _canMove = true;
    
    private void OnEnable()
    {
        InputManager.movement += CalculateVelocity;
        InputManager.walkStarted += StartWalking;
        InputManager.walkCanceled += StopWalking;
    }

    private void FixedUpdate()
    {
        if (_canMove && !_walking)
            transform.Translate(new Vector3(_movementDirection.x, _movementDirection.y,0) * (_speed * Time.deltaTime));
        
        if (_canMove && _walking)
            transform.Translate(new Vector3(_movementDirection.x, _movementDirection.y,0) * (_walkSpeed * Time.deltaTime));
    }

    private void CalculateVelocity(Vector2 move)
    {
        _movementDirection.y = move.y;
        _movementDirection.x = move.x;

        AnimationState();
    }

    private void AnimationState()
    {
        if (_walking)
            PlayerAnimation.instance.AnimationStateSwitcher(_state, _movementDirection);
        else
        {
            _state = _movementDirection.y switch
            {
                1 => 0,
                -1 => 1,
                _ => _state
            };

            _state = _movementDirection.x switch
            {
                -1 => 2,
                1 => 3,
                _ => _state
            };
        
            PlayerAnimation.instance.AnimationStateSwitcher(_state, _movementDirection);
        }
    }
    
    private void StartWalking(InputAction.CallbackContext context)
    {
        _walking = true;
        StartCoroutine(WhileWalking());
    }

    private void StopWalking(InputAction.CallbackContext context)
    {
        _walking = false;
    }
    
    private void MovementSwitch() => _canMove = !_canMove;

    private void CheckCell()
    {
        var cell = PlayerFarming.instance.DetermineCell();

        if (cell.y != -1)
        {
           Debug.Log(cell);
        }
        else return;
    }
    
    private IEnumerator WhileWalking()
    {
        while (_walking)
        {
            yield return new WaitForEndOfFrame();
            CheckCell();
        }
    }
}
