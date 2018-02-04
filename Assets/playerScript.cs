using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour {
    //  Player Movement Variables
    public float playerMoveSpeed = 6;
    public float playerJumpSpeed = 18;
    float playerClimbSpeed = 0.02f;
    
    // Player State variables
    bool facingRight = true;
    public bool facingLadder = false;
    public bool isClimbing = false;
    
    // Access to Unity Components
    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidBody;
    private Animator animator;
    
    // Player on Ground Variables
    public Transform groundCheck;
    public float groundCheckRadius = 0.09f;
    public LayerMask whatIsGround;
    public bool isGrounded;

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
        setAnimatorState();
    }
    private void handleInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerMoveRight();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerMoveLeft();
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            playerClimbUp();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            playerClimbDown();
        }
        if (Input.GetKey(KeyCode.Space)&& (isGrounded == true))
        {
            PlayerJump();
        }
    }

    private void setAnimatorState()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("moveSpeed", Mathf.Abs(rigidBody.velocity.x));
        
        if (currentPlayerState == CurrentPlayerState.climbing)
        {
            //rigidBody.velocity = new Vector3(0, 0, 0);
            animator.SetBool("isClimbing", true);
            Debug.Log("climbing");
        }
        else if (currentPlayerState == CurrentPlayerState.alive)
        {
            animator.SetBool("isClimbing", false);
        }
    }

    private void PlayerJump()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, playerJumpSpeed, 0);
    }

    private void playerMoveRight()
    {
    if (isClimbing == false)
        {
            if (facingRight == true)
            {
            rigidBody.velocity = new Vector3(playerMoveSpeed, rigidBody.velocity.y, 0);
            }
            else
            {
                spriteRenderer.flipX = true;
                rigidBody.velocity = new Vector3(playerMoveSpeed, rigidBody.velocity.y, 0);
                facingRight = true;
            }
        }
    }
    private void playerMoveLeft()
    {
        if (facingRight == true)
        {
            spriteRenderer.flipX = false;
            rigidBody.velocity = new Vector3(-playerMoveSpeed, rigidBody.velocity.y, 0);
            facingRight = false;
        }
        else
        {
            rigidBody.velocity = new Vector3(-playerMoveSpeed, rigidBody.velocity.y, 0);
        }

    }
    private void playerClimbUp()
    {

        switch (currentPlayerState)
        {
            case CurrentPlayerState.alive:
                if (facingLadder == true)
                {
                    currentPlayerState = CurrentPlayerState.climbing;
                }
                break;
            case CurrentPlayerState.climbing:
                rigidBody.gravityScale = 0;
                playerPosition = transform.position;
                //transform.position = new Vector3(playerPosition.x, playerPosition.y+0.02f,0);
                transform.position = new Vector3(2.1f, playerPosition.y +playerClimbSpeed, 0);
                break;
            default:
                break;


        }
    }

    private void playerClimbDown()
    {
        switch (currentPlayerState)
        {
            case CurrentPlayerState.climbing:
               // if (facingLadder == true && isGrounded == true)
               // {
                  //  currentPlayerState = CurrentPlayerState.alive;
                  //  rigidBody.gravityScale = 5;
               // }
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
