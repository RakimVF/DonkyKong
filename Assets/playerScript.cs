using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour {
    //  Player Movement Variables
    [SerializeField] float playerMoveSpeed = 6;
    [SerializeField] float playerJumpSpeed = 18;
    [SerializeField] float playerClimbSpeed = 0.02f;
    float horizontalMovement;
    float verticalMovement;

    // Player State variables
    bool facingRight = true;
    [SerializeField] bool facingLadder = false;
    [SerializeField] bool isClimbing = false;
    
    // Access to Unity Components
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    private Animator animator;

    // Player on Ground Variables
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.09f;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] bool isGrounded;

    // Player State
    enum CurrentPlayerState { alive,dead,climbing}
    CurrentPlayerState currentPlayerState;

    // Pixel perfect Ladder Movement
    Vector2 playerPosition; 


    // Use this for initialization
    void Start ()
    {

        currentPlayerState = CurrentPlayerState.alive;
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        setAnimatorState();
        handleInput();
    }



    void FixedUpdate ()
    {
        switch (currentPlayerState)
        {
            case CurrentPlayerState.alive:
            rigidBody.gravityScale = 5;
            break;
        }


        handleInput();
        
    }
    private void handleInput()
    {
        // Get Input Axis
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");
        // Horizontal Move
        playerMoveHorizontal(horizontalMovement);




        if (verticalMovement > 0)
            {
            playerClimbUp();
        }
        else if (verticalMovement < 0)
        {
            playerClimbDown();
        }
        if (Input.GetKeyDown(KeyCode.Space)&& (isGrounded == true))
        {
            PlayerJump();
        }
    }


    private void playerMoveHorizontal(float horizontalMovement)
    {
        // Check direction player is facing and flip accordingly
        checkAndFlipPlayer(horizontalMovement);
        // accelerate the player in the direction given in horizontalMovement
        rigidBody.velocity = new Vector2(horizontalMovement * playerMoveSpeed, rigidBody.velocity.y);
    }

    private void playerClimbUp()
    {

        switch (currentPlayerState)
        {
            case CurrentPlayerState.alive:
                if (facingLadder == true)
                {
                    currentPlayerState = CurrentPlayerState.climbing;
                    setAnimatorState();
                }
                break;
            case CurrentPlayerState.climbing:
                rigidBody.gravityScale = 0;
                playerPosition = transform.position;
                //transform.position = new Vector3(playerPosition.x, playerPosition.y+0.02f,0);
                transform.position = new Vector3(2.1f, playerPosition.y + playerClimbSpeed, 0);
                break;
            default:
                break;



        }
        setAnimatorState();
    }

    private void playerClimbDown()
    {
        switch (currentPlayerState)
        {
            case CurrentPlayerState.climbing:
                if (facingLadder == true)
                {
                    playerPosition = transform.position;

                    transform.position = new Vector3(2.1f, playerPosition.y - playerClimbSpeed, 0);
                }
                break;
            default:
                if (facingLadder == true)
                {
                    playerPosition = transform.position;

                    transform.position = new Vector3(2.1f, playerPosition.y - playerClimbSpeed, 0);
                }
                break;
        }

    }

    private void PlayerJump()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, playerJumpSpeed);
    }

    private void setAnimatorState()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("moveSpeed", Mathf.Abs(rigidBody.velocity.x));
        
        if (currentPlayerState == CurrentPlayerState.climbing)
        {
            animator.SetBool("isClimbing", true);
            Debug.Log("climbing");
        }
        else if (currentPlayerState == CurrentPlayerState.alive)
        {
            animator.SetBool("isClimbing", false);
        }
    }



    private void checkAndFlipPlayer (float horizontalMovement)
    {
        if (horizontalMovement < 0 && facingRight == true)
        {
            spriteRenderer.flipX = false;
            facingRight = false;
        }
        else if (horizontalMovement > 0 && facingRight == false)
        {
            spriteRenderer.flipX = true;
            facingRight = true;
        }
    }
   

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            facingLadder = true;

        }
        if (other.gameObject.tag == "upperLadder")
        {
            //animator.Play("playerMovingOffLadder");
            if (currentPlayerState == CurrentPlayerState.climbing)
            {
                currentPlayerState = CurrentPlayerState.alive;  
                rigidBody.gravityScale = 5;
            }


        }
        currentPlayerState = CurrentPlayerState.alive;
        rigidBody.gravityScale = 5;
        setAnimatorState();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            facingLadder = false;
            rigidBody.gravityScale = 5;
            currentPlayerState = CurrentPlayerState.alive;

        }
    }


}
