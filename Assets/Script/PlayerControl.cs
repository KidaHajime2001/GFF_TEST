using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
public class PlayerControl : MonoBehaviour, PlayerAct.IPlayerActionActions
{
    [Tooltip("�L�����N�^�[�R���g���[���[")]
    [SerializeField] CharacterController characterController;

    [Tooltip("�ړ������x�N�g��")]
    Vector3 targetDirection;

    [Tooltip("�ړ����x")]
    float speed = 3.0f;

    [Tooltip("���肩�ǂ����̃t���O")]
    bool runFlag;

    [Tooltip("�ݒu����̋��󃌃C�̔��a")]
    [SerializeField] float groundCheckRadius = 0.1f;

    [Tooltip("")]
    [SerializeField] float groundCheckOffsetY = 0.0f;

    [Tooltip("")]
    [SerializeField] float groundCheckDistance = 0.2f;

    [Tooltip("���C���[�}�X�N�F�ڒn�Ɏg��")]
    [SerializeField] LayerMask groundLayers = 0;

    [Tooltip("���C�̃q�b�g")]
    RaycastHit hit;

    [Tooltip("���X�ɉ�]�����邽�߂̖ڕWQuaternion")]
    Quaternion targetRot;

    PlayerAct.PlayerActionActions input;
    
    [Tooltip("�W�����v�t���O�@false=�W�����v���Ă��Ȃ�,true=�W�����v���Ă���")]
    bool jumpFlag;

    [Tooltip("�������x")]
    float fallSpeed = 5.0f;

    [Tooltip("�����̉����x")]
    float verticalVelocity;

    [Tooltip("�W�����v�̗�")]
    float jumpPower = 5.0f;

    float gravity = -7.0f;

    float animationBlend;


    float WALK_SPEED = 30.0f;
    float RUN_SPEED = 100.0f;
    float SPEED_CHANGE_RATE = 10.0f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    private float rotationVelocity;

    [Tooltip("�A�j���[�V�����̊Ǘ��ɕK�v")]
    Animator animator;

    int animIDSpeed;
    int animIDGround;
    int animIDJump;
    int animIDFreeFall;

    float JUMP_COOL_TIME = 0.5f;
    float coolTimeCountStart;
    bool jumpCoolTimeFlag=false;

    void Awake()
    {
        // �C���v�b�g�𐶐����āA���g���R�[���o�b�N�Ƃ��ēo�^
        input = new PlayerAct.PlayerActionActions(new PlayerAct());
        input.SetCallbacks(this);
        animator = GetComponent<Animator>();
        SetAnimationID();
    }

    void OnEnable() => input.Enable();
    // �C���v�b�g�̗L���E������
    void OnDestroy() => input.Disable();
    void OnDisable() => input.Disable();
    void Update()
    {
        Move();
        CalculationGravity();
        //Debug.Log("velo"+ verticalVelocity);
        if (CheckGroundStatus())
        {
            Debug.Log("���n");
            
            if(jumpFlag)
            {
                jumpCoolTimeFlag = true;
                coolTimeCountStart = Time.time;
            }
            jumpFlag = false;
            if(animator)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);
                animator.SetBool(animIDGround, true);
            }

        }
        else
        {
            animator.SetBool(animIDGround, false);
            animator.SetBool(animIDFreeFall, true);
        }
        if (jumpCoolTimeFlag&&JUMP_COOL_TIME<Time.time-coolTimeCountStart)
        {
            jumpCoolTimeFlag = false;
        }

    }

    void SetAnimationID()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDGround = Animator.StringToHash("Ground");
        animIDJump = Animator.StringToHash("Jump");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("jump");
        if(!jumpFlag&&!jumpCoolTimeFlag)
        {
            verticalVelocity = jumpPower;
            jumpFlag = true;
            if(animator)
            {
                animator.SetBool(animIDJump,true);
            }
        }

    }
    void CalculationGravity()
    {
        if(!CheckGroundStatus())
        {
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -fallSpeed;
            }
        }

        verticalVelocity += gravity * Time.deltaTime;


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //���͂��x�N�g����
        targetDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;

        //���͂Ȃ��̑΍�
        if (targetDirection.magnitude <= 0.1f) targetDirection = Vector3.zero;

        

    }
    void Move()
    {
        
        float targetSpeed = runFlag ? RUN_SPEED : WALK_SPEED;
        if (targetDirection == Vector3.zero)
        {
            targetSpeed = 0;
        }
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SPEED_CHANGE_RATE);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;

        }
        else
        {
            speed = targetSpeed;
        }
        
        animationBlend = Mathf.Lerp(animationBlend,targetSpeed,Time.deltaTime * SPEED_CHANGE_RATE);
        if (animator)
        {

            //Debug.Log("Speed:" + animationBlend);
            animator.SetFloat(animIDSpeed, animationBlend);
        }
        //�ړ������։�]
        AdjustDirection();



        characterController.Move(targetDirection.normalized * (speed * Time.deltaTime) +
            new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime
            );
    }


    private void AdjustDirection()
    {
        if (targetDirection != Vector3.zero)
        {
            var targetRotation = Mathf.Atan2(targetDirection.x,targetDirection.z) * Mathf.Rad2Deg +Camera.main.transform.eulerAngles.y;
            float mRotation= Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, mRotation, 0.0f);

        }

      
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            runFlag = true;
        }
        else if(context.canceled)
        {
            runFlag= false;
        }
        

    }
    bool CheckGroundStatus()
    {
        return Physics.CheckSphere(transform.position + groundCheckOffsetY * Vector3.up,
            groundCheckRadius,
            groundLayers);
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = CheckGroundStatus() ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position + groundCheckOffsetY * Vector3.up, groundCheckRadius);
    }
}
