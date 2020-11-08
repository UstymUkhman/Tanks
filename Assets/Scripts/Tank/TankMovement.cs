using UnityEngine;


public class TankMovement : MonoBehaviour
{
    public int playerNumber = 1;

    public float pitchRange = 0.2f;
    public float speed      = 12.0f;
    public float turnSpeed  = 180.0f;

    public AudioClip   jumpingClip;
    public AudioClip   engineIdling;
    public AudioClip   engineDriving;
    public AudioSource movementAudio;

    private bool canJump;

    private float maxDamage;
    private float originalPitch;
    private float turnInputValue;
    private float movementInputValue;

    private string jumpButton;
    private string fireButton;
    private string turnAxisName;
    private string movementAxisName;

    private new Rigidbody rigidbody;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    private void OnEnable ()
    {
        turnInputValue        = 0.0f;
        movementInputValue    = 0.0f;
        maxDamage             = 25.0f;
        rigidbody.isKinematic = false;
        // isKinematic == forces will be applied to RigidBody
    }

    private void OnDisable ()
    {
        rigidbody.isKinematic = true;
    }
    
    private void Start()
    {
        jumpButton       = "Jump"       + playerNumber;
        fireButton       = "Fire"       + playerNumber;
        turnAxisName     = "Horizontal" + playerNumber;
        movementAxisName = "Vertical"   + playerNumber;

        originalPitch        = movementAudio.pitch;
        movementAudio.clip   = engineIdling;
        movementAudio.volume = 0.0f;
        movementAudio.loop   = true;
        canJump              = true;

        movementAudio.Play();
        EngineAudio();
    }

    private void Update()
    {
        if (Input.GetButtonUp(fireButton))
        {
            movementAudio.pitch  = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
            movementAudio.clip   = engineIdling;
            movementAudio.loop   = true;
            movementAudio.volume = 0.0f;
            movementAudio.Play();
        }

        if (Input.GetButtonDown(jumpButton) && canJump)
        {
            rigidbody.AddForce(transform.up * 5.0f, ForceMode.VelocityChange);

            movementAudio.pitch = originalPitch;
            movementAudio.clip = jumpingClip;
            movementAudio.loop = false;
            movementAudio.volume = 1.0f;

            movementAudio.Play();
            canJump = false;
        }

        EngineAudio();

        turnInputValue     = Input.GetAxis(turnAxisName);
        movementInputValue = Input.GetAxis(movementAxisName);
    }

    private void EngineAudio()
    {
        bool isJumping = movementAudio.clip == jumpingClip;
        bool isIdling  = movementAudio.clip == engineIdling;

        bool isTurning = Mathf.Abs(turnInputValue)     < 0.1f;
        bool isMoving  = Mathf.Abs(movementInputValue) < 0.1f;

        if (isMoving && isTurning && canJump)
        {
            movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
            movementAudio.clip = engineIdling;
            movementAudio.volume = 0.0f;
            movementAudio.loop = true;
            movementAudio.Play();
        }
        else if ((!isMoving || !isTurning) && (isIdling || isJumping) && canJump)
        {
            movementAudio.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
            movementAudio.clip = engineDriving;
            movementAudio.volume = 0.5f;
            movementAudio.loop = true;
            movementAudio.Play();
        }
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void OnCollisionEnter(Collision other)
    {
        canJump = true;

        if (!other.gameObject.CompareTag("Ground"))
        {
            float damage = 0.0f;
            TankHealth targetHealth = rigidbody.GetComponent<TankHealth>();
            
            if (targetHealth)
            {
                damage = CalculateDamage();
                targetHealth.TakeDamage(damage);
            }

            if (other.gameObject.CompareTag("Player"))
            {
                TankHealth otherTankHealth = other.gameObject.GetComponent<TankHealth>();

                if (otherTankHealth)
                {
                    otherTankHealth.TakeDamage(damage * 1.5f);
                }
            }
        }
    }

    private void Move()
    {
        Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + movement);
    }
    
    private void Turn()
    {
        float turnY = turnInputValue * turnSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.Euler(0.0f, turnY, 0.0f);
        rigidbody.MoveRotation(rigidbody.rotation * rotation);
    }

    private float CalculateDamage()
    {
        float damage = movementInputValue * speed * Time.deltaTime;
        return Mathf.Max(0.0f, damage * maxDamage);
    }
}