using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using Unity.Collections;

public class Player : MonoBehaviour
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
    private BoxCollider2D boxCollider;
    private Vector2 boxColliderSize;
    private RaycastHit2D hit;
    private Rigidbody2D rb;
    private float checkDistanceY;
    private float move;
    private float waitBeforeMove = 0;
    private float inAir = 0;
    public float fallingTime = 0;
    private bool startRun = false;
    private bool isRunning = false;
    public bool isGrounded = true;
    private bool allowMove = true;

    [Header("Tags & Layers")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask platformLayer;
    public bool fallThrought;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer animationSprite;
    [SerializeField] private Animator animator;
    private const string WALK = "Walk";
    private const string IDLE = "Idle";
    private const string FALL = "Fall";
    private const string LAND = "Land";
    private const string JUMP_START = "JumpStart";
    private const string JUMP_MAX = "JumpMax";
    private const string RUN = "Run";
    private bool mirrorAnim;
    private string currentState = "";
    private short jumpState;
    private short oldJumpState;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxColliderSize = new Vector2(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y);
        checkDistanceY = boxCollider != null ? boxColliderSize.y / 2 + 0.01f: 1f;
        ChangeAnimationState(IDLE);
    }

    private void Update()
    {
        if (move != 0) {
            mirrorAnim = move <= 0;
        }

        animationSprite.flipX = mirrorAnim;
    }

    private void FixedUpdate() {
        AnimationHandle();

        if (waitBeforeMove > 0) {
            allowMove = false;
            waitBeforeMove -= Time.deltaTime;
            if (waitBeforeMove < 0)
                waitBeforeMove = 0;
        } else {
            allowMove = true;
        }
    
        hit = Physics2D.BoxCast(transform.position, new Vector2(boxColliderSize.x, 0.01f), 0f, Vector2.down, checkDistanceY, groundLayer);
        if (hit.collider == null) {
            hit = Physics2D.BoxCast(transform.position, new Vector2(boxColliderSize.x, 0.01f), 0f, Vector2.down, checkDistanceY, platformLayer);
        }
    
        if (hit.collider != null && Math.Round(rb.velocity.y, 1) == 0) {
            if (isGrounded)
                inAir = 0;
            isGrounded = true;
            jumpsRemaining = maxJumpsInAir;
        } else {
            isGrounded = false;
            inAir += Time.fixedDeltaTime;
        }

        ///////////////////////
        if (!(move != 0 && allowMove && isGrounded)) {
            rb.velocity = new Vector2(Lerp(rb.velocity.x, 0, 0.001f), rb.velocity.y);
        }
        if (isGrounded) {
            if (allowMove || jumpState != 4) {
                jumpState = 0;
            }
            if (move != 0 && allowMove) {
                
                if (Math.Round(Math.Abs(rb.velocity.x), 3) > walkSpeed) {
                    isRunning = true;
                } else {
                    isRunning = false;
                }
                float targetVelocityX = startRun || isRunning ? runSpeed * move : walkSpeed * move;
                if (targetVelocityX * move == runSpeed) {
                    if(move > 0)
                        rb.velocity = new Vector2(Math.Max(Lerp(rb.velocity.x, targetVelocityX, 0.05f), walkSpeed * move), rb.velocity.y);
                    else if (move < 0)
                        rb.velocity = new Vector2(Math.Min(Lerp(rb.velocity.x, targetVelocityX, 0.05f), walkSpeed * move), rb.velocity.y);
                } else {
                    rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);
                }
            } else {
                rb.velocity = new Vector2(Lerp(rb.velocity.x, 0, 0.3f), rb.velocity.y);
            }
        } else {

            if (rb.velocity.y >= -0.25f && rb.velocity.y <= 0.25f) {
                jumpState = 2;
                fallingTime = 0;
            }
            else if (rb.velocity.y < -0.3f) {
                jumpState = 3;
                fallingTime = fallingTime + Time.fixedDeltaTime;
            }
            else if (rb.velocity.y > 0.3f) {
                jumpState = 1;
                fallingTime = 0;
            } else {
                jumpState = 0;
            }
            if (move > 0) {
                rb.velocity = new Vector2(Math.Max(Lerp(rb.velocity.x, airSpeed * move, 0.03f), rb.velocity.x), rb.velocity.y);
            }
            else if (move < 0) {
                rb.velocity = new Vector2(Math.Min(Lerp(rb.velocity.x, airSpeed * move, 0.03f), rb.velocity.x), rb.velocity.y);
            }
            if (Math.Round(rb.velocity.x, 3) > airSpeed) {
                rb.velocity = new Vector2(Math.Max(Lerp(rb.velocity.x, airSpeed, 0.1f), rb.velocity.x), rb.velocity.y);
            }
            else if (Math.Round(rb.velocity.x, 3) < -airSpeed) {
                rb.velocity = new Vector2(Math.Min(Lerp(rb.velocity.x, -airSpeed, 0.1f), rb.velocity.x), rb.velocity.y);
            }
        }
        if (oldJumpState != jumpState) {
            if (jumpState == 0 && oldJumpState == 3) {
                if (fallingTime > 1) {
                    waitBeforeMove = 0.75f; 
                    jumpState = 4;
                }
                fallingTime = 0;
            }
            oldJumpState = jumpState;
        }
    }

    private void AnimationHandle() {
        if (move != 0 && jumpState == 0) {
            if (isRunning) {
                ChangeAnimationState(RUN);
                if (Math.Abs(rb.velocity.x) > walkSpeed) {
                    animator.SetFloat("Speed", Math.Abs(rb.velocity.x)/10);
                }
                else {
                    animator.SetFloat("Speed", 1f);
                }
            } else {
                ChangeAnimationState(WALK);
                animator.SetFloat("Speed", 1f);
            }
        } else {
            animator.SetFloat("Speed", 1f);
            if (jumpState == 0) {
                ChangeAnimationState(IDLE);
            } else {
                if (jumpState == 1) {
                    ChangeAnimationState(JUMP_START);
                } else if (jumpState == 2) {
                    ChangeAnimationState(JUMP_MAX);
                } else if (jumpState == 3) {
                    ChangeAnimationState(FALL);
                } else if (jumpState == 4) {
                    ChangeAnimationState(LAND);
                }
            }
        }
    }

    private void ChangeAnimationState(string newState) {
        if(currentState == newState) 
            return;
        
        animator.Play(newState);

        currentState = newState;
    }

    public void MoveEvent(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>().x;
    }

    public void JumpEvent(InputAction.CallbackContext context) {
        if (context.performed) {
            if (jumpsRemaining > 0 && allowMove) {
                if (!isGrounded) {
                    rb.AddForce(new Vector2(0, jumpForce-rb.velocity.y+0.3f), ForceMode2D.Impulse);
                } else {
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                }
                if (!isGrounded)
                    jumpsRemaining--;
            }
        }
        else if (context.canceled) {
            if (rb.velocity.y > 0.3f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    public void RunEvent(InputAction.CallbackContext context) {
        if (context.performed) {
            startRun = true;
        } else if (context.canceled) {
            startRun = false;
        }
    }
    public void PlatformEvent(InputAction.CallbackContext context) {
        if (context.performed) {
            fallThrought = true;
        } else if(context.canceled) {
            fallThrought = false;
        }
    }
    private static float Lerp(float firstValue, float secondValue, float by) {
        return firstValue * (1 - by) + secondValue * by;
    }
}