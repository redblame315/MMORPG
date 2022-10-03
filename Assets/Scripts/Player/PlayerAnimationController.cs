using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Require these components when using this script
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerAnimationController : NetworkBehaviour
{
    public float animSpeed = 1.0f;				        // a public setting for overall animator animation speed
    public bool useCurves;

    private Animator animator;							// a reference to the animator on the character
    private AnimatorStateInfo currentBaseState;         // a reference to the current state of the animator, used for base layer
    private PlayerCharacterController controller;       // a reference to the player controller script

    float verticalVelocity;                             // setup variable for our vertical input axis
    float horizontalVelocity;                           // setup variable for our horizontal input axis
    bool jumping = false;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int forwardLocoState = Animator.StringToHash("Base Layer.Forward Locomotion");
    static int backwardLocoState = Animator.StringToHash("Base Layer.Backward Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");

	// Use this for initialization
	void Start () 
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerCharacterController>();
	}

    void FixedUpdate()
    {            
        animator.SetFloat("VSpeed", verticalVelocity);                          // set our animator's float parameter 'VSpeed' equal to the vertical input axis
        animator.SetFloat("HSpeed", horizontalVelocity);                        // set our animator's float parameter 'HSpeed' equal to the horizontal input axis
        animator.SetBool("Jump", jumping);
        animator.speed = animSpeed;                                             // set the speed of our animator to the public variable 'animSpeed'
        currentBaseState = animator.GetCurrentAnimatorStateInfo(0);	            // set our currentState variable to the current state of the Base Layer (0) of animation
    }

	// Update is called once per frame
	void Update () 
    {
        if(isLocalPlayer)
        {
            verticalVelocity = Input.GetAxis("Vertical");
            horizontalVelocity = Input.GetAxis("Horizontal");

            if (controller.moveState == PlayerEnums.MovementState.Walking || controller.moveState == PlayerEnums.MovementState.WalkingRight || controller.moveState == PlayerEnums.MovementState.WalkingLeft)
            {
                verticalVelocity = verticalVelocity * 0.5f;
                horizontalVelocity = horizontalVelocity * 0.5f;
            }

            if (currentBaseState.nameHash == idleState)
            {
                if (Input.GetButton("Jump"))
                {
                    jumping = true;
                }
            }
            else if (currentBaseState.nameHash == jumpState)
            {
                
            }
            else
            {
                if(!animator.IsInTransition(0))
                {
                    jumping = false;
                }
            }
        }
        else
        {
            // Normalize vertical and horizontal components to values between -1 and 0
        }

        // Debug.Log(verticalVelocity.ToString() + " | " + horizontalVelocity.ToString());
	}
}
