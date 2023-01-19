using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    Rigidbody playerRigidbody;
    
    //movement variables
    private float playerSpeed = 0f;
    public float playerMaxSpeed;
    public float playerSpeedIncrement;
    public float playerMinSpeed;



    //vertical movement variables
    float xMove, zMove;
    bool isMoving;
    Vector3 movementDirection;
    private bool isGrounded;
    private float lastGroundedTime = -1f;
    public float jumpHoldDuration = 0.5f;
    public float playerJumpForce;
    public float gravityForce;  //determines fall speed
    private float groundSphereRadius;
    private Vector3 groundSpherePos;

    //camera variables
    Camera cameraMain;
    Vector3 cameraForward;
    Vector3 cameraRight;


    // Start is called before the first frame update
    void Start()
    {
        cameraMain = Camera.main;
        movementDirection = new Vector3(0, 0, 0);
        isGrounded = true;
        playerSpeed = playerMinSpeed;
        playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Physics.gravity = new Vector3(0, -gravityForce, 0);

        Movement();
       

    }

    void OnGUI()
    {

        GUI.Label(new Rect(10, 10, 100, 20), "Speed: " + playerSpeed);
        GUI.Label(new Rect(10, 40, 100, 20), "X Input: " + Input.GetAxisRaw("Horizontal"));
        GUI.Label(new Rect(10, 80, 100, 20), "Y Input: " + Input.GetAxisRaw("Vertical"));
        GUI.Label(new Rect(10, 120, 100, 20), "Grounded: " + isGrounded);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundSpherePos, groundSphereRadius);
    }

    public void Jump()
    {
        GroundSphereCast(); //used to determine whether the player is on the ground or not

        if (isMoving)
            movementDirection = new Vector3(xMove, 0, zMove).normalized;    //Determines the direction the input is pointing

        if (isGrounded)
        {   
            lastGroundedTime = Time.time;   //the time you left the ground
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(movementDirection), Time.deltaTime * 40f);   //Resets player rotation
        }
        else
            transform.Rotate(Time.deltaTime * 2000, 0, 0, Space.Self);  //Causes player to SPIN

        //once you leave the ground, you can influence the height of your jump by 'jumpHoldDuration' seconds to allow for variable height
        if (Input.GetKey(KeyCode.Space) && Time.time < lastGroundedTime + jumpHoldDuration)
        {
            playerRigidbody.useGravity = false; //jumping ignores the influence of gravity for 'jumpHoldDuration' time
            playerRigidbody.velocity += new Vector3(0, playerJumpForce, 0);

        }
        else
            playerRigidbody.useGravity = true;
    }

    public void Movement()
    {
        xMove = Input.GetAxisRaw("Horizontal"); // d key changes value to 1, a key changes value to -1
        zMove = Input.GetAxisRaw("Vertical"); // w key changes value to 1, s key changes value to -1
        
        //gets the direction of the camera
        cameraForward = cameraMain.transform.forward;
        cameraRight = cameraMain.transform.right;

        var desiredMoveDirection = cameraForward * zMove + cameraRight * xMove;

        if (xMove != 0 || zMove != 0)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving)   //if player is moving
        {
            // If the player is below the max speed, increases it by 'playerSpeedIncrement'
            if (playerSpeed < playerMaxSpeed)
                playerSpeed += playerSpeedIncrement * Time.deltaTime;
            else
                playerSpeed = playerMaxSpeed;   //prevents going over max speed
        }
        else // If the player is not moving
        {
            // Decreases the speed by an incremental amount until reaching minimum starting speed
            if (playerSpeed > playerMinSpeed)
                playerSpeed -= playerSpeedIncrement * Time.deltaTime;
            else
                playerSpeed = playerMinSpeed;   //prevents going under minimum speed
        }

        playerRigidbody.velocity += desiredMoveDirection * playerSpeed; //updates speed by adding to velocity

        Jump();
    }

    public void GroundSphereCast()
    {
        groundSphereRadius = this.GetComponent<CapsuleCollider>().radius * 1.9f;    //radius of the capsule collider, but a little bigger
        groundSpherePos = transform.position + Vector3.down * (groundSphereRadius * 1.9f);  //the position of the collider, below the player's feet
        isGrounded = Physics.CheckSphere(groundSpherePos, groundSphereRadius, LayerMask.GetMask("Ground")); //determines if the player is one any object with the layer ground
    }
}
