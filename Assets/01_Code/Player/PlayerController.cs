using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [Header("Checks")]
    [SerializeField] private bool isSliding;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumpReady;
    [SerializeField] private float raycastLength = 1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float groundDrag = 1f;

    [Header("Sprint")]
    [SerializeField] private float sprintMulitplier = 3f;
    [SerializeField] private Vector3 WeaponSprintRotation = new Vector3(3f, -75f, 4f);
    [SerializeField] private Vector3 WeaponSpawnRotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private float WeaponRotationTransitionSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float jumpCooldown = 0.25f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 0.6f;
    [SerializeField] private float standHeight = 1f;
    [SerializeField] private float crouchSpeed = 2f;

    [Header("Reference")]
    [SerializeField] private PlayerCameraController playerCameraController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputHelper inputHelper;
    [SerializeField] private GameObject WeaponHolder;
    [SerializeField] private GameObject Collider;
    private float headHeight;
    private Vector3 WeaponRotation;

    [Header("Weapon Selector")]
    [SerializeField] private WeaponSelector playerWeaponSelector;

    Vector3 currentDirection;
    private Vector3 movementValue;
    private float verticalRotation;

    private float SpeedValue => inputHelper.IsCrouching ? crouchSpeed : (inputHelper.IsSprinting ? walkSpeed * sprintMulitplier : walkSpeed);

    Rigidbody rb;
    CapsuleCollider capsuleCollider => Collider.GetComponent<CapsuleCollider>();
    ShootConfigScript shootConfigScript => playerWeaponSelector.ActiveWeapon.ShootConfig;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.freezeRotation = true;
        rb.useGravity = true;

        if (playerWeaponSelector.ActiveWeapon != null)
        {
            playerWeaponSelector.ActiveWeapon.Spawn(WeaponHolder.transform, this);
        }
    }

    void FixedUpdate()
    {
        Crouching();
        ApplyGroundDrag();
        CheckInputs();
        Movement();
        Shooting();
    }

    private void Update()
    {
        SprintRotationWeapon();
        CheckGrounded();
        ControlSpeed();
    }

    private void ApplyGroundDrag()
    {
        Vector3 v = rb.linearVelocity;
        float dragFactor = 1f - groundDrag * Time.fixedDeltaTime;
        v.x *= dragFactor;
        v.z *= dragFactor;
        rb.linearVelocity = new Vector3(v.x, v.y, v.z);
    }

    private void CheckInputs()
    {
        if (inputHelper.IsJumping && isJumpReady && isGrounded)
        {
            isJumpReady = false;
            Jumping();
            Invoke(nameof(ResetJumping), jumpCooldown);
        }
    }

    private void Movement()
    {
        currentDirection = playerCameraController.orientation.forward * inputHelper.MoveInput.y + playerCameraController.orientation.right * inputHelper.MoveInput.x;
        rb.AddForce(currentDirection.normalized * SpeedValue, ForceMode.Force);
    }

    private void ControlSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > SpeedValue)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * SpeedValue;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Jumping()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJumping()
    {
        isJumpReady = true;
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, raycastLength, groundLayer);
        Debug.DrawRay(transform.position, Vector3.down * raycastLength, isGrounded ? Color.green : Color.red);
    }

    private void Shooting()
    {
        if (inputHelper.IsAttacking && playerWeaponSelector.ActiveWeapon != null)
        {
            playerWeaponSelector.ActiveWeapon.Shoot();
        }
    }

    private void Crouching()
    {
        bool forceCrouch = HasObstacleAboveHead();
        bool finalCrouch = inputHelper.IsCrouching || forceCrouch;
        float targetYScale = finalCrouch ? crouchHeight : standHeight;

        Vector3 currentScale = capsuleCollider.transform.localScale;
        float newYScale = Mathf.MoveTowards(currentScale.y, targetYScale, standHeight * Time.deltaTime);
        capsuleCollider.transform.localScale = new Vector3(currentScale.x, newYScale, currentScale.z);
    }

    private bool HasObstacleAboveHead()
    {
        Debug.DrawRay(transform.position, Vector3.up * raycastLength, Color.blue);
        return Physics.Raycast(transform.position, Vector3.up, standHeight);
    }

    public void Recoil()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(playerCamera.transform.localRotation.eulerAngles.x - shootConfigScript.Recoil,
                                                                  playerCamera.transform.localRotation.eulerAngles.y + shootConfigScript.Recoil,
                                                                  playerCamera.transform.localRotation.eulerAngles.z);
    }



    public void SprintRotationWeapon()
    {
        if (WeaponHolder == null)
        {
            Debug.LogWarning("WeaponHolder is NULL, skipping SprintRotation");
            return;
        }
        Quaternion targetRotation = inputHelper.IsSprinting
            ? Quaternion.Euler(WeaponSprintRotation)
            : Quaternion.Euler(WeaponSpawnRotation);
        WeaponHolder.transform.localRotation = Quaternion.Lerp(
            WeaponHolder.transform.localRotation,
            targetRotation,
            WeaponRotationTransitionSpeed * Time.deltaTime
        );
    }
}