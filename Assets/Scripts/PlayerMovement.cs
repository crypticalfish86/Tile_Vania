using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    private Vector2 moveInput; //user input to move player
    private Rigidbody2D myRigidBody; //player physics

    [SerializeField] float runSpeed = 8f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        FlipSprite();
    }

    //When player jumps (Key = Spacebar)
    private void OnJump(InputValue value){
        Debug.Log("Jumped");
    }

    private void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }
        private void Run(){
            Vector2 playerVelocity = new Vector2 (runSpeed * moveInput.x, myRigidBody.velocity.y); //keeps velocity the same (so if you have gravity on it stays the same)
            myRigidBody.velocity = playerVelocity; //rigidbody.velocity is the speed in a certain direction of the physics of your object
        }
        
        private void FlipSprite(){
            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

            if (playerHasHorizontalSpeed){
                transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x), 1f);
            }
        }
}
