using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    [SerializeField] Transform neck;
    [SerializeField] Transform graphicsRoot;
    [SerializeField] Transform eyes;
    //[SerializeField] Transform rearRoot;
    [SerializeField] float rotationMultX = 1f;
    [SerializeField] float rotationMultY = 1f;
    [SerializeField] float headTiltMaxX = 80f;
    [SerializeField] bool invertLookY = false;

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float pounceForce = 20f;

    [SerializeField] LayerMask groundLayers;

    float jumpBuffer = 0.2f;

    Rigidbody rigid;

    Vector2 move;
    Vector2 look;
    float jump = 0f;
    [HideInInspector]
    public float horizontal, vertical, lookHorizontal, lookVertical;

    bool isGrounded;
    Vector3 floorNormal = Vector3.up;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if(pauseMenu.activeSelf) return;

        horizontal = move.x;
        vertical = move.y;
        lookHorizontal = look.x;
        lookVertical = look.y;

        RotateNeck();

        LookForTarget();
    }

    void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        if(jump > 0f)
        {
            jump -= Time.deltaTime;
            if(isGrounded) 
            {
                jump = 0f;
                Jump();
            }
        }

        if (Mathf.Abs(vertical) > 0 || Mathf.Abs(horizontal) > 0)
        {
            Vector3 moveDirection = Vector3.ProjectOnPlane(neck.forward, floorNormal).normalized * vertical
                            + Vector3.ProjectOnPlane(neck.right, floorNormal).normalized * horizontal;
                    
            moveDirection = moveDirection * moveSpeed * Time.deltaTime;

            rigid.AddForce(moveDirection, ForceMode.VelocityChange);

            //ROTATE REAR TO FOLLOW MOVE DIRECTION
        }
        else
        {
            if(isGrounded && rigid.velocity.magnitude > 1f)
            {
                rigid.AddForce(-rigid.velocity * Time.deltaTime * 5f, ForceMode.VelocityChange);
            }
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

    void LookForTarget()
    {
        RaycastHit hit;
        if(Physics.Raycast(eyes.position, eyes.forward, out hit, 50))
        {
            if(hit.collider.tag == "Enemy")
            {
                print("PLAYER IS SEEING A MOUSE!");
            }
        }
    }


    void Jump()
    {
        //Debug.Log("SHOULD BE JUMPING");
        rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Pounce()
    {
        if(!isGrounded)
            rigid.AddForce(neck.forward * pounceForce, ForceMode.Impulse);
        else rigid.AddForce(Vector3.ProjectOnPlane(neck.forward, floorNormal).normalized * pounceForce, ForceMode.Impulse);
    }


    bool CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + Vector3.up * 0.6f, 0.5f, Vector3.down, out hit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore))
        {
            floorNormal = hit.normal;
            return true;
        }

        floorNormal = Vector3.up;
        return false;
    }


    //Input Events
    void OnLook(InputValue value)
    {
        if (pauseMenu.activeSelf) return;

        look = value.Get<Vector2>();
    }
    void OnMove(InputValue value)
    {
        if (pauseMenu.activeSelf) return;

        move = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (pauseMenu.activeSelf) return;

        if (value.isPressed)
            jump = jumpBuffer;
    }
    void OnAttack(InputValue value)
    {
        if (pauseMenu.activeSelf) return;

        if(!value.isPressed) return;

        Debug.Log("SWIPE OR SOMETHING");
        Pounce();
    }
    void OnPause(InputValue value)
    {
        if(!value.isPressed) return;

        if(Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Unpause();
        }
    }

    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }


    //HELPER FUNCTIONS
    public static float WrapAngle(float angle)
    {
        if (angle > 180) return angle - 360f;
        if (angle < -180) return angle + 360f;
        return angle;
    }
}
