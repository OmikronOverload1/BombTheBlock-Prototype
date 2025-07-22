using UnityEngine;
using UnityEngine.InputSystem;

public class InputHelper : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string attack = "Attack";
    [SerializeField] private string reload = "Reload";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string crouch = "Crouch";



    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsReloading { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool IsCrouching { get; private set; }


    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction attackAction;
    private InputAction reloadAction;
    private InputAction interactAction;
    private InputAction crouchAction;


    public void Awake()
    {
        InputActionMap mapReference = playerControls.FindActionMap(actionMapName);

        moveAction = mapReference.FindAction(move);
        lookAction = mapReference.FindAction(look);
        jumpAction = mapReference.FindAction(jump);
        sprintAction = mapReference.FindAction(sprint);
        attackAction = mapReference.FindAction(attack);
        reloadAction = mapReference.FindAction(reload);
        interactAction = mapReference.FindAction(interact);
        crouchAction = mapReference.FindAction(crouch);

        InputActionsToActionEvent();
    }

    private void InputActionsToActionEvent()
    {
        moveAction.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => MoveInput = Vector2.zero;

        lookAction.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => LookInput = Vector2.zero;

        jumpAction.performed += ctx => IsJumping = true;
        jumpAction.canceled += ctx => IsJumping = false;

        sprintAction.performed += ctx => IsSprinting = true;
        sprintAction.canceled += ctx => IsSprinting = false;

        attackAction.performed += ctx => IsAttacking = true;
        attackAction.canceled += ctx => IsAttacking = false;

        reloadAction.performed += ctx => IsReloading = true;
        reloadAction.canceled += ctx => IsReloading = false;

        interactAction.performed += ctx => IsInteracting = true;
        interactAction.canceled += ctx => IsInteracting = false;

        crouchAction.performed += ctx => IsCrouching = true;
        crouchAction.canceled += ctx => IsCrouching = false;
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName).Disable();
    }

    public void SetCrouching(bool value)
    {
        IsCrouching = value;
    }
}
