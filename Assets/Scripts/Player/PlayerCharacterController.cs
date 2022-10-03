/// <summary> 
/// Charactercontrollercs.cs 
/// Character Controller in CSharp v2.3 
/// </summary> 
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCharacterController : NetworkBehaviour
{ 
             
    //Var definition 
    public bool isSwimming = false;                    //Can be triggert to slow down the movements (like when u swim) 
    public PlayerEnums.MovementState moveState = PlayerEnums.MovementState.Idle;
     
    //Movement speeds 
    private float jumpSpeed = 6.0f;                  //Jumpspeed / Jumpheight 
    private float gravity = 20.0f;                   //Gravity for jump 
    private float runSpeed = 10.0f;                  //Speed when the Character is running 
    private float walkSpeed = 4.0f;                  //Speed when the Character is walking (normal movement) 
    private float rotateSpeed = 250.0f;              //Rotationspeed of the Character 
    private float walkBackMod = 0.75f;               //Speed in Percent for walk backwards and sidewalk 
     
    //Internal vars to work with 
    private float speedMod = 0.0f;                   //temp Var for Speedcalculation 
    private bool grounded = false;                   //temp var if the character is grounded 
    private Vector3 moveDirection = Vector3.zero;    //move direction of the Character 
    private bool isWalking = false;                  //toggle var between move and run 
    private bool isJumping = false;                    //temp var for jumping 
    private bool mouseSideButton = false;            //temp var for mouse side buttons 
    private float pbuffer = 0.0f;                    //Cooldownpuffer for SideButtons 
    private float coolDown = 0.5f;                   //Cooldowntime for SideButtons 
    private CharacterController controller;          //CharacterController for movement 
     
    void Start()
    {
        //Get CharacterController 
        controller = GetComponent<CharacterController>();
    }

    //Every Frame 
    void Update()
    {
        if (isLocalPlayer)
        {
            //Set idel animation 
            moveState = PlayerEnums.MovementState.Idle;
            isWalking = true;

            // Hold "Run" to run 
            if (Input.GetAxis("Run") != 0)
                isWalking = false;

            // Only allow movement and jumps while grounded 
            if (grounded)
            {
                //movedirection 
                moveDirection = new Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));

                //pushbuffer to avoid on/off flipping 
                if (pbuffer > 0)
                    pbuffer -= Time.deltaTime;
                if (pbuffer < 0) pbuffer = 0;

                //Automove Sidebuttonmovement 
                if ((Input.GetAxis("Toggle Move") != 0) && pbuffer == 0)
                {
                    pbuffer = coolDown;
                    mouseSideButton = !mouseSideButton;
                }
                if (mouseSideButton && ((Input.GetAxis("Vertical") != 0) || Input.GetButton("Jump")) || (Input.GetMouseButton(0) && Input.GetMouseButton(1)))
                    mouseSideButton = false;

                //L+R MouseButton Movement 
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1) || mouseSideButton)
                    moveDirection.z += 1;
                if (moveDirection.z > 1)
                    moveDirection.z = 1;

                //Strafing move (like Q/E movement     
                moveDirection.x -= Input.GetAxis("Strafing");

                // if moving forward and to the side at the same time, compensate for distance 
                if (Input.GetMouseButton(1) && (Input.GetAxis("Horizontal") != 0) && (Input.GetAxis("Vertical") != 0))
                {
                    moveDirection *= 0.7f;
                }

                //Speedmodification / is moving forward or side/backward 
                speedMod = ((Input.GetAxis("Vertical") < 0) || (Input.GetMouseButton(1) && (Input.GetAxis("Horizontal")) != 0) || Input.GetAxis("Strafing") != 0) ? walkBackMod : 1.0f;

                //Use run or walkspeed 
                moveDirection *= isWalking ? walkSpeed * speedMod : runSpeed * speedMod;

                //reduce movement by 70% when swimming is toggled    
                moveDirection *= isSwimming ? 0.7f : 1;

                // Jump! 
                if (Input.GetButton("Jump"))
                {
                    isJumping = true;
                    moveDirection.y = jumpSpeed;
                }

                //moveState normal movement (for animations)               
                if ((moveDirection.x == 0) && (moveDirection.z == 0))
                    moveState = PlayerEnums.MovementState.Idle;
                if (moveDirection.z > 0)
                    moveState = isWalking ? PlayerEnums.MovementState.Walking : PlayerEnums.MovementState.Running;
                if (moveDirection.z < 0)
                    moveState = isWalking ? PlayerEnums.MovementState.WalkingBack : PlayerEnums.MovementState.RunningBack;
                if (moveDirection.x > 0)
                    moveState = isWalking ? PlayerEnums.MovementState.WalkingRight : PlayerEnums.MovementState.RunningRight;
                if (moveDirection.x < 0)
                    moveState = isWalking ? PlayerEnums.MovementState.WalkingLeft : PlayerEnums.MovementState.RunningLeft;

                //moveState swim movement (for animations)               
                if (isSwimming)
                {
                    if ((moveDirection.x == 0) && (moveDirection.z == 0))
                        moveState = PlayerEnums.MovementState.SwimIdle;
                    if (moveDirection.z > 0)
                        moveState = isWalking ? PlayerEnums.MovementState.Swimming : PlayerEnums.MovementState.SwimmingFast;
                    if (moveDirection.z < 0)
                        moveState = isWalking ? PlayerEnums.MovementState.SwimmingBack : PlayerEnums.MovementState.SwimmingBackFast;
                    if (moveDirection.x > 0)
                        moveState = isWalking ? PlayerEnums.MovementState.SwimmingRight : PlayerEnums.MovementState.SwimmingFastRight;
                    if (moveDirection.x < 0)
                        moveState = isWalking ? PlayerEnums.MovementState.SwimmingLeft : PlayerEnums.MovementState.SwimmingFastLeft;
                    if (isJumping)
                        moveState = PlayerEnums.MovementState.SwimmingUp;
                }

                //transform direction.
                moveDirection = transform.TransformDirection(moveDirection);
            }
            // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down. 
            if (Input.GetMouseButton(1))
            {
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }
            else
            {
                transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            }

            //Apply gravity 
            moveDirection.y -= gravity * Time.deltaTime;

            //Move Charactercontroller and check if grounded 
            grounded = ((controller.Move(moveDirection * Time.deltaTime)) & CollisionFlags.Below) != 0;

            //Reset jumping after landing 
            isJumping = grounded ? false : isJumping;

            //moveState jump/swimup (for animations)       
            if (isJumping)
            {
                moveState = PlayerEnums.MovementState.Jumping;
                Debug.Log("Jumping!");
            }
                
            if (isJumping && isSwimming)
            {
                moveState = PlayerEnums.MovementState.SwimmingUp;
            }
        }
        else
        {
            // handle network player details
        }

        // Debug.Log(moveState.ToString());
    }
} 