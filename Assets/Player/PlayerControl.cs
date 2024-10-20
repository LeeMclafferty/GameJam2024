using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    [SerializeField] Transform neck;
    [SerializeField] Transform graphicsRoot;
    [SerializeField] Transform eyes;
    [SerializeField] Transform leftArm;
    [SerializeField] Transform rightArm;
    [SerializeField] Transform[] legs = new Transform[4];
    //[SerializeField] Transform rearRoot;
    [SerializeField] float rotationMultX = 1f;
    [SerializeField] float rotationMultY = 1f;
    [SerializeField] float headTiltMaxX = 80f;
    [SerializeField] bool invertLookY = false;

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float pounceForce = 20f;
    [SerializeField] float pounceTime = 0.5f;
    [SerializeField] float grabSpeed = 0.5f;
    [SerializeField] float stepSpeed = 0.2f;
    [SerializeField] float stepHeight = 0.3f;

    [SerializeField] LayerMask groundLayers;

    public UnityEvent onMouseGrab;

    float jumpBuffer = 0.2f;

    Rigidbody rigid;

    Vector2 move;
    Vector2 look;
    float jump = 0f;
    float pounce = 0f;
    [HideInInspector]
    public float horizontal, vertical, lookHorizontal, lookVertical;

    bool isGrounded, grabbingMouse, isPouncing;

    float stepTime = 0f;
    int stepIndex = 0;

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

        if(pounce > 0f)
        {
            pounce -= Time.deltaTime;
            if (pounce <= 0f)
            {
                if(!isGrounded)
                {
                    pounce = Time.deltaTime;
                }
                else
                {
                    if (!grabbingMouse)
                    {
                        leftArm.gameObject.SetActive(false);
                        rightArm.gameObject.SetActive(false);
                        legs[0].gameObject.SetActive(true);
                        legs[1].gameObject.SetActive(true);
                    }
                    isPouncing = false;
                }
            }
        }
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

            if(!isPouncing && !grabbingMouse)
                AnimateLegs();
            
        }
        else
        {
            if(isGrounded && rigid.velocity.magnitude > 1f)
            {
                rigid.AddForce(-rigid.velocity * Time.deltaTime * 5f, ForceMode.VelocityChange);
            }
        }
    }

    void AnimateLegs()
    {
        stepTime += Time.deltaTime;
        legs[stepIndex].localPosition = new Vector3(0, Mathf.Sin((stepTime / stepSpeed) * Mathf.PI) * stepHeight, 0);
        legs[stepIndex + 2].localPosition = new Vector3(0, Mathf.Sin((stepTime / stepSpeed) * Mathf.PI) * stepHeight, 0);
        if (stepTime > stepSpeed)
        {
            stepTime = 0f;
            legs[stepIndex].localPosition = Vector3.zero;
            legs[stepIndex + 2].localPosition = Vector3.zero;
            stepIndex++;
            stepIndex %= 2;
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
        if(isPouncing || grabbingMouse)
            return;

        isPouncing = true;
        if(!isGrounded)
            rigid.AddForce(neck.forward * pounceForce, ForceMode.Impulse);
        else rigid.AddForce(Vector3.ProjectOnPlane(neck.forward, floorNormal).normalized * pounceForce, ForceMode.Impulse);

        leftArm.gameObject.SetActive(true);
        rightArm.gameObject.SetActive(true);
        legs[0].gameObject.SetActive(false);
        legs[1].gameObject.SetActive(false);
        pounce = pounceTime;
    }

    IEnumerator GrabMouse()
    {
        grabbingMouse = true;
        leftArm.gameObject.SetActive(true);
        rightArm.gameObject.SetActive(true);
        legs[0].gameObject.SetActive(false);
        legs[1].gameObject.SetActive(false);

        float time = 0f;
        while(time < grabSpeed)
        {
            time += Time.deltaTime;
            leftArm.localRotation = Quaternion.Euler(0, Mathf.Lerp(0f, 25f, time/grabSpeed),0);
            rightArm.localRotation = Quaternion.Euler(0, Mathf.Lerp(0f, -25f, time / grabSpeed), 0);

            yield return null;
        }

        leftArm.localRotation = Quaternion.identity;
        rightArm.localRotation = Quaternion.identity;
        leftArm.gameObject.SetActive(false);
        rightArm.gameObject.SetActive(false);
        legs[0].gameObject.SetActive(true);
        legs[1].gameObject.SetActive(true);
        grabbingMouse = false;
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


    void OnTriggerEnter(Collider other)
    {
        MouseControler mouse = other.GetComponent<MouseControler>();
        if (mouse != null)
        {
            Debug.Log("PLAYER CAUGHT A MOUSE");
            onMouseGrab.Invoke();
            // TODO destroy mouse or deactivate collider and put in front of player's face while grabbing, then destroy
            if(!grabbingMouse)
                StartCoroutine(GrabMouse());
        }
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
