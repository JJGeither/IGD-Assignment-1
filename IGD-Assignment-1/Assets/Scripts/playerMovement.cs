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

    private int ringCount;

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
    private Vector3 desiredMoveDirection;

    //camera variables
    Camera cameraMain;
    Vector3 cameraForward;
    Vector3 cameraRight;

    public string collectableTag;

    public GameObject jumpingModel;
    public GameObject standingModel;
    public GameObject currentModel;

    // Start is called before the first frame update
    void Start()
    {
        cameraMain = Camera.main;
        movementDirection = new Vector3(0, 0, 0);
        isGrounded = true;
        playerSpeed = playerMinSpeed;
        currentModel = standingModel;
        ringCount = 0;
        //playerRigidbody = this.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        Physics.gravity = new Vector3(0, -gravityForce, 0);

        ChangeModel();
        playerRigidbody = this.GetComponent<Rigidbody>();

        Movement();
       

    }

    void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.fontSize = 30;
        guiStyle.normal.textColor = Color.yellow;

        GUIStyle ringStyle = new GUIStyle();
        ringStyle.fontSize = 30;
        ringStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 100, 20), "Rings ", guiStyle);
        GUI.Label(new Rect(130, 10, 100, 20),"" + ringCount, ringStyle);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundSpherePos, groundSphereRadius);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == collectableTag)
        {
            Destroy(other.gameObject);
            ringCount++;
        }
    }

    public void ChangeModel()
    {
        if (!isGrounded)
        {
            standingModel.transform.position = jumpingModel.transform.position;

            jumpingModel.SetActive(true);
            standingModel.SetActive(false);
            currentModel = jumpingModel;
        } else
        {
            jumpingModel.transform.position = standingModel.transform.position;
            jumpingModel.SetActive(false);
            standingModel.SetActive(true);
            currentModel = standingModel;
        }
    }
    public void Jump()
    {
        GroundSphereCast(); //used to determine whether the player is on the ground or not

        if (isMoving)
        {
            movementDirection = desiredMoveDirection;    //Determines the direction the input is pointing
            movementDirection.y = 0;
        }


 

        //once you leave the ground, you can influence the height of your jump by 'jumpHoldDuration' seconds to allow for variable height
        if (Input.GetKey(KeyCode.Space) && Time.time < lastGroundedTime + jumpHoldDuration)
        {
            playerRigidbody.useGravity = false; //jumping ignores the influence of gravity for 'jumpHoldDuration' time
            playerRigidbody.velocity += new Vector3(0, playerJumpForce, 0);
            isGrounded = false;

        }
        else
            playerRigidbody.useGravity = true;


        if (isGrounded)
        {
            lastGroundedTime = Time.time;   //the time you left the ground
            currentModel.transform.rotation = Quaternion.Slerp(currentModel.transform.rotation, Quaternion.LookRotation(movementDirection), Time.deltaTime * 40f);   //Resets player rotation
        }
        else
        {
            currentModel.transform.Rotate(Time.deltaTime * 2000, 0, 0, Space.Self);  //Causes player to SPIN

        }
            

    }

    public void Movement()
    {



        xMove = Input.GetAxisRaw("Horizontal"); // d key changes value to 1, a key changes value to -1
        zMove = Input.GetAxisRaw("Vertical"); // w key changes value to 1, s key changes value to -1
        
        //gets the direction of the camera
        cameraForward = cameraMain.transform.forward;
        cameraRight = cameraMain.transform.right;

        desiredMoveDirection = cameraForward * zMove + cameraRight * xMove;

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
                playerSpeed -= playerSpeedIncrement * Time.deltaTime * 2;
            else
                playerSpeed = playerMinSpeed;   //prevents going under minimum speed
        }

        playerRigidbody.velocity += desiredMoveDirection * playerSpeed; //updates speed by adding to velocity

        Jump();
    }

    public void GroundSphereCast()
    {
        groundSphereRadius = currentModel.GetComponent<CapsuleCollider>().height;    //radius of the capsule collider, but a little bigger
        groundSphereRadius = .5f;
        groundSpherePos = currentModel.transform.position + (Vector3.down * 1.3f) ;  //the position of the collider, below the player's feet
        isGrounded = Physics.CheckSphere(groundSpherePos, groundSphereRadius, LayerMask.GetMask("Ground")); //determines if the player is one any object with the layer ground
    }
}
