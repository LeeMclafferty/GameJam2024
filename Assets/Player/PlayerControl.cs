using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Transform neck;
    [SerializeField] Transform graphicsRoot;
    [SerializeField] float rotationMultX = 1f;
    [SerializeField] float rotationMultY = 1f;
    [SerializeField] float headTiltMaxX = 80f;
    [SerializeField] bool invertLookY = false;

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    [SerializeField] LayerMask groundLayers;

    Rigidbody rigid;

    Vector2 move;
    Vector2 look;
    bool jump;
    [HideInInspector]
    public float horizontal, vertical, lookHorizontal, lookVertical;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }


    void Update()
    {
        horizontal = move.x;
        vertical = move.y;
        lookHorizontal = look.x;
        lookVertical = look.y;

        RotateNeck();
    }

    void FixedUpdate()
    {
        bool isGrounded = CheckGrounded();
        if(jump)
        {
            jump = false;
            if(isGrounded) Jump();
        }

        if (Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0)
        {
            Vector3 moveDirection = Vector3.ProjectOnPlane(neck.forward, Vector3.up).normalized * vertical
                            + Vector3.ProjectOnPlane(neck.right, Vector3.up).normalized * horizontal;
                    
            moveDirection = moveDirection * moveSpeed * Time.deltaTime;

            rigid.AddForce(moveDirection, ForceMode.VelocityChange);
        }
    }


    void RotateNeck()
    {
        Vector3 newRot = neck.localRotation.eulerAngles;
        newRot.x -= lookVertical * rotationMultX;
        if(invertLookY)
            newRot.x *= -1;
        newRot.x = WrapAngle(newRot.x);
        newRot.x = Mathf.Clamp(newRot.x, -headTiltMaxX, headTiltMaxX);

        newRot.y += lookHorizontal * rotationMultY;
        newRot.y = WrapAngle(newRot.y);

        neck.localRotation = Quaternion.Euler(newRot);

        //ROTATE GRAPHICS ROOT TO MATCH LOOK DIRECTION
        graphicsRoot.localRotation = Quaternion.Euler(0, newRot.y, 0);
    }

    void Jump()
    {
        Debug.Log("SHOULD BE JUMPING");
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    bool CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore))
        {

            return true;
        }

        return false;
    }


    //Input Events
    void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }
    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (value.isPressed)
            jump = true;
    }
    void OnAttack(InputValue value)
    {
        Debug.Log("SWIPE OR SOMETHING");
        //pounce or swipe at mouse or something
    }


    //HELPER FUNCTIONS
    public static float WrapAngle(float angle)
    {
        if (angle > 180) return angle - 360f;
        if (angle < -180) return angle + 360f;
        return angle;
    }
}
