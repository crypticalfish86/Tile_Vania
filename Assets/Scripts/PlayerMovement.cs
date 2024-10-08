using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class PlayerMovement : MonoBehaviour
{
    
    private Vector2 moveInput; //user input to move player (x direction: a = -1, d = 1. y direction s = -1, w = 1)
    private PlayerInput playerInputComponent;
    private Rigidbody2D myRigidBody; //player physics
        private float defaultGravity; //default gravity (don't start scene with player on ladder or this breaks)
    private BoxCollider2D myFeetCollider; //player feet collider
    private Animator playerAnimator;//the animator of the player

    //death and enemies
    private bool isAlive;

    LayerMask enemyLayerMask;

    [Header("player movement")]
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float jumpSpeed = 4f;
    [SerializeField] float climbSpeed = 1f;
    [SerializeField] Vector2 deathKickSpeed = new Vector2 (10f, 10f);

    [Header("Firing Mechanism")]
    [SerializeField] GameObject bow;
    [SerializeField] GameObject arrow;

    private bool currentlyShooting; //bool to determine if player is currently firing arrow
    private bool currentlyJumping; //bool to determine if player is in the air/jumping

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        playerInputComponent = GetComponent<PlayerInput>();
            playerInputComponent.ActivateInput();//activate controls
        myRigidBody = GetComponent<Rigidbody2D>();
            defaultGravity = myRigidBody.gravityScale;
        myFeetCollider = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponent<Animator>();
        currentlyShooting = false;//start being able to shoot

        enemyLayerMask = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerMask, false); //restart collision between player and enemy layer
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive){
            Run();
            FlipSprite();
            ClimbLadder();
        }
    }

    //When player jumps, makes player jump(Key = Spacebar)
    private void OnJump(InputValue value){
        //if input value key pressed, and player is touching ground then jump
        if(value.isPressed && (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))){
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
        private void FlipSprite() {
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; //if absolute velocity value is greater than near 0 value(Mathf.Epsilon)

            if (playerHasHorizontalSpeed){
                transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x), 1f); //sprite scale is either -1 or 1 due to Mathf.Sign which just determines +ve/-ve value
            }
        }

    //Add y velocity and turn off gravity to player when player comes in contact with climbing ladder
    private void ClimbLadder() {

        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){
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

    /*
        Begin Coroutine to Instantiate/Fire an arrow/bullet from the bow position.
        Coroutine plays the shooting animation, then instantiates an arrow at the exact moment in the animation that the bow fires
        then switches bool to false allowing player to idle again
    */
    private void OnFire(InputValue value) {
        if (value.isPressed && !currentlyShooting && !currentlyJumping){
            StartCoroutine(ShootArrow());
        }
    }
        private IEnumerator ShootArrow() {
            currentlyShooting = true;
            playerAnimator.SetBool("isShooting", true);
            yield return new WaitForSeconds(0.3f);
            Instantiate(arrow, bow.transform.position, Quaternion.identity);
            playerAnimator.SetBool("isShooting", false);
            yield return new WaitForSeconds(0.4f);
            currentlyShooting = false;
        }

    /*
        when touching enemy kill player:
            -set alive status to false
            -turn off collision between enemy layer and player layer
            -run Die()
    */
    private void OnCollisionEnter2D(Collision2D other) {
        currentlyJumping = false;
        if (other.gameObject.tag == "Enemy" && isAlive){
            isAlive = false;
            Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayerMask, true);//turn off collision between enemy and player to prevent further collision
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        currentlyJumping = true;
    }
    
    /*#
        on death:
            -deactivate player controls
            -set death animation
            -reset gravity to default
            -kick player in the air for death effect
    */
    private void Die(){
        if (!isAlive){
            playerInputComponent.DeactivateInput();
            playerAnimator.SetTrigger("Death");
            myRigidBody.gravityScale = defaultGravity;
            
            //depending on if player is facing left or right, the death kick speed kicks the player sprite in different x directions
            if (transform.localScale.x > 0){
                myRigidBody.velocity = deathKickSpeed;
            }
            else{
                deathKickSpeed.x = -deathKickSpeed.x;
                myRigidBody.velocity = deathKickSpeed;
            }

            StartCoroutine(ReloadLevel());
        }
    }
        private IEnumerator ReloadLevel(){
            yield return new WaitForSeconds(2f);
            FindObjectOfType<GameSessionController>().ProcessPlayerDeath();
        }
}
