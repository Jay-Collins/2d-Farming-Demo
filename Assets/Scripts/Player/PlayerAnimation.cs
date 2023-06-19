using UnityEngine;

public class PlayerAnimation : MonoSingleton<PlayerAnimation>
{
    private enum AnimationState {WalkUp = 0, WalkDown = 1, WalkLeft = 2, WalkRight = 3}
    
    [Header("References")] 
    [SerializeField] private Animator _animator;

    private AnimationState _animationState;
    private readonly int _walkState = Animator.StringToHash("WalkState");
    private readonly int _speed = Animator.StringToHash("Speed");
    private readonly int _handsFull = Animator.StringToHash("HandsFull");
    private readonly int _usedPickaxe = Animator.StringToHash("UsedPickaxe");
    private readonly int _usedOtherTool = Animator.StringToHash("UsedOtherTool");

    public void AnimationStateSwitcher(int state, Vector2 movementDirection)
    {
        _animationState = state switch
        {
            0 => AnimationState.WalkUp,
            1 => AnimationState.WalkDown,
            2 => AnimationState.WalkLeft,
            3 => AnimationState.WalkRight,
            _ => _animationState
        };
        
        // set animation parameters
        _animator.SetFloat(_speed, movementDirection.sqrMagnitude);
        _animator.SetFloat(_walkState, (float)_animationState);
    }

    public void SetHandsAnimation(bool hands) => _animator.SetBool(_handsFull, hands);

    public void UsedPickaxe() => _animator.SetTrigger(_usedPickaxe);
    
    public void UsedOtherTool() => _animator.SetTrigger(_usedOtherTool);
    
    public int CheckDirection() => (int)_animationState;
}

