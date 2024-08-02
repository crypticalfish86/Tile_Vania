using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerMovement : MonoBehaviour
{
    
    private Vector2 moveInput; //user input to move player (x direction: a = -1, d = 1. y direction s = -1, w = 1)
    private Rigidbody2D myRigidBody; //player physics
        private float defaultGravity; //default gravity (don't start scene with player on ladder or this breaks)
    private Collider2D myCollider; //player collider
    private Animator playerAnimator;//the animator of the player

    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpSpeed = 4f;
    [SerializeField] float climbSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
            defaultGravity = myRigidBody.gravityScale;
        myCollider = GetComponent<Collider2D>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    //When player jumps, makes player jump(Key = Spacebar)
    private void OnJump(InputValue value){
        Debug.Log("Jumped");
        //if input value key pressed, and player is touching ground then jump
        if(value.isPressed && myCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            myRigidBody.velocity += new Vector2 (0, jumpSpeed);
        }
    }

    //changes the value of moveInput (Keys = WASD)
    private void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }
        //adds velocity in the x direction to our player object
        private void Run(){
            Vector2 playerVelocity = new Vector2 (runSpeed * moveInput.x, myRigidBody.velocity.y); //keeps velocity the same (so if you have gravity on it stays the same)
            myRigidBody.velocity = playerVelocity; //rigidbody.velocity is the speed in a certain direction of the physics of your object

            //if player absolute velocity value is greater than near 0, set running animation, otherwise unset it
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; //if absolute velocity value is greater than near 0 value(Mathf.Epsilon)
            if(playerHasHorizontalSpeed){
                playerAnimator.SetBool("isRunning", true);
            }
            else{
                playerAnimator.SetBool("isRunning", false);
            }
        }
        //flips player sprite when moving right or left
        private void FlipSprite(){
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; //if absolute velocity value is greater than near 0 value(Mathf.Epsilon)

            if (playerHasHorizontalSpeed){
                transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x), 1f); //sprite scale is either -1 or 1 due to Mathf.Sign which just determines +ve/-ve value
            }
        }

    //Add y velocity and turn off gravity to player when player comes in contact with climbing ladder
    private void ClimbLadder(){

        if (myCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){
            myRigidBody.gravityScale = 0f;

            Vector2 playerVelocity = new Vector2 (myRigidBody.velocity.x, climbSpeed * moveInput.y);
            myRigidBody.velocity = playerVelocity;

            //if player is climbing player climbing animation, if idling on ladder play idle animation
            if(myRigidBody.velocity.y != 0){
                playerAnimator.SetBool("isClimbing", true);
            }
            else{
                playerAnimator.SetBool("isClimbing", false);
            }
        }
        else{
            myRigidBody.gravityScale = defaultGravity;
            playerAnimator.SetBool("isClimbing", false);
        }
    }
}
