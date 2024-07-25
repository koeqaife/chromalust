using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Collections;
using System.Collections;

public class OldPlayer : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 12;
    [SerializeField] private short maxJumpsInAir = 1;
    private short jumpsRemaining;

    [Header("Movement")]
    [SerializeField] private int walkSpeed = 5;
    [SerializeField] private int airSpeed = 3;
    [SerializeField] private int runSpeed = 12;
    //
    private bool allowMove = true;
    private BoxCollider2D boxCollider;
    private float move;
    private float checkDistanceY;
    private Vector2 boxColliderSize;
    private RaycastHit2D hit;
    private Rigidbody2D rb;
    private float waitBeforeMove = 0;
    private float inAir = 0;
    private float fallingTime = 0;
    private bool run = false;
    private bool isRunning = false;
    private bool isGround = true;


    [Header("Tags & Layers")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask platformLayer;
    private bool _fallThrought;
    public bool fallThrought;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer animationSprite;
    [SerializeField] private Animator animator;
    private bool mirrorAnim;
    private short currentState = 0;
    public static short State;

    public float testvar = 1.2f;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxColliderSize = new Vector2(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y);
        checkDistanceY = boxCollider != null ? boxColliderSize.y / 2 + 0.01f: 1f;
    }

    private static float Lerp(float firstValue, float secondValue, float by) {
        return firstValue * (1 - by) + secondValue * by;
    }

    void Update() {
        if (isGround && _fallThrought) {
            fallThrought = true;
        }
        else {
            fallThrought = false;
        }
    }

    void FixedUpdate() {
        if (waitBeforeMove > 0) {
            allowMove = false;
            waitBeforeMove -= Time.deltaTime;
            if (waitBeforeMove < 0)
                waitBeforeMove = 0;
        }
        else {
            allowMove = true;
        }
        animationSprite.flipX = mirrorAnim;

        if (move != 0) {
            mirrorAnim = move <= 0;
        }

        hit = Physics2D.BoxCast(transform.position, new Vector2(boxColliderSize.x, 0.01f), 0f, Vector2.down, checkDistanceY, groundLayer);
        if (hit.collider == null) {
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxColliderSize.x, 0.01f), 0f, Vector2.down, checkDistanceY, platformLayer);
        }
        if (hit.collider != null && Math.Round(rb.velocity.y, 1) == 0) {
            if (isGround)
                inAir = 0;
            isGround = true;
            jumpsRemaining = maxJumpsInAir;
        }
        else {
            isGround = false;
            inAir += Time.fixedDeltaTime;
        }

        if (isGround) {
            if (move != 0 && allowMove) {
                if (run && (currentState == 1 || currentState == 0)) {
                    isRunning = true;
                }
                if (isRunning && Math.Abs(rb.velocity.x) > walkSpeed) {
                    currentState = 2;
                }
                float targetVelocityX = isRunning ? runSpeed * move : (currentState == 1 || currentState == 0) ? walkSpeed * move : -999;
                if (targetVelocityX != -999)
                    if (isRunning && move > 0)
                        rb.velocity = new Vector2(Math.Max(Lerp(rb.velocity.x, targetVelocityX, 0.05f), walkSpeed * move), rb.velocity.y);
                    else if (isRunning && move < 0)
                        rb.velocity = new Vector2(Math.Min(Lerp(rb.velocity.x, targetVelocityX, 0.05f), walkSpeed * move), rb.velocity.y);
                    else
                        rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
                if (currentState == 0 && Math.Abs(rb.velocity.x) > 0)
                    currentState = 1;
                if (currentState == 5 || currentState == 4 || currentState == 3) 
                    currentState = 0;
            }
            else {
                rb.velocity = new Vector2(Lerp(rb.velocity.x, 0, 0.2f), rb.velocity.y);
                currentState = 0;
            }
        }
        else {
            if (rb.velocity.y >= -0.25f && rb.velocity.y <= 0.25f) {
                currentState = 4;
            }
            else if (rb.velocity.y < -0.3f) {
                currentState = 5;
                fallingTime = fallingTime + Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0.3f && currentState != 4 && currentState != 5) {
                currentState = 3;
            }
            if (move > 0) {
                rb.velocity = new Vector2(Math.Max(Lerp(rb.velocity.x, airSpeed * move, 0.03f), rb.velocity.x), rb.velocity.y);
            }
            else if (move < 0) {
                rb.velocity = new Vector2(Math.Min(Lerp(rb.velocity.x, airSpeed * move, 0.03f), rb.velocity.x), rb.velocity.y);
            }
        }
        if (currentState == 0 && !allowMove)
            allowMove = true;
        if (!isGround && currentState != 3 && currentState != 4 && currentState != 5)
            Debug.LogError(currentState);
        State = currentState;
        if ((currentState == 1 || currentState == 2) && rb.velocity.x == 0) {
            currentState = 0;
        }
        if (isRunning && currentState != 2) {
            isRunning = false;
        }
        if (currentState == 3 && fallingTime > 0) {
            fallingTime = 0;
        }
        if (currentState != 5 && currentState != 3 && currentState != 4) {
            if (fallingTime > 1) {
                waitBeforeMove = 1;
            }
            fallingTime = 0;
        }
        Animation();
    }
    void Animation() {
        animator.SetInteger("State", currentState);
        animator.SetFloat("WaitBeforeMove", waitBeforeMove);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGround", isGround);
        if (isRunning && Math.Abs(rb.velocity.x) > walkSpeed) {
            animator.SetFloat("Speed", Math.Abs(rb.velocity.x)/10);
        }
        else {
            animator.SetFloat("Speed", 1f);
        }
    }
    public void MoveEvent(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>().x;
    }
    public void JumpEvent(InputAction.CallbackContext context) {
        if (context.performed) {
            if (jumpsRemaining > 0 && allowMove) {
                if (!isGround) {
                    rb.AddForce(new Vector2(0, jumpForce-rb.velocity.y+0.3f), ForceMode2D.Impulse);
                }
                else {
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                }
                currentState = 3;
                if (!isGround)
                    jumpsRemaining--;
            }
        }
        else if (context.canceled) {
            if (rb.velocity.y > 0.3f && currentState != 5)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    public void RunEvent(InputAction.CallbackContext context) {
        if (context.performed) {
            run = true;
        }
        else if (context.canceled) {
            run = false;
        }
    }
    public void PlatformEvent(InputAction.CallbackContext context) {
        if (context.performed) {
            _fallThrought = true;
        }
        else if(context.canceled) {
            _fallThrought = false;
        }
    }

    public static float CalculateFallTime(float distance, float gravity, float gravityScale)
    {
        gravity *= gravityScale;
        return Mathf.Sqrt(2 * distance / -gravity);
    }
    public static float CalculateFallTime(float distance, float gravity, float gravityScale, float initialVelocity)
    {
        gravity *= gravityScale;
        return Mathf.Sqrt((2 * distance - initialVelocity) / -gravity)-0.01f;
    }
}
