using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public Slider aimSlider;
    public Rigidbody shellBody;
    public Transform fireTransform;

    public AudioClip fireClip;
    public AudioSource shootingAudio;

    public int playerNumber = 1;

    public float minLaunchForce = 15f;
    public float maxLaunchForce = 30f;
    public float maxChargeTime = 0.75f;


    private bool fired;
    private string fireButton;
    private float chargeSpeed;
    private float currentLaunchForce;


    private void OnEnable()
    {
        currentLaunchForce = minLaunchForce;
        aimSlider.value    = minLaunchForce;
    }

    private void Start()
    {
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        fireButton  = "Fire" + playerNumber;
    }

    private void Update()
    {
        if (currentLaunchForce >= maxLaunchForce && !fired)
        {
            currentLaunchForce = maxLaunchForce;
        }

        if (Input.GetButtonUp(fireButton) && !fired)
        {
            aimSlider.value = minLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(fireButton))
        {
            currentLaunchForce   = minLaunchForce;
            fired = false;
        }
        else if (Input.GetButton(fireButton) && !fired)
        {
            currentLaunchForce += chargeSpeed * Time.deltaTime;
            aimSlider.value = currentLaunchForce;
        }
    }

    private void Fire()
    {
        Rigidbody shell = Instantiate(shellBody, fireTransform.position, fireTransform.rotation) as Rigidbody;
        shell.velocity = currentLaunchForce * fireTransform.forward;

        currentLaunchForce   = minLaunchForce;
        shootingAudio.clip   = fireClip;
        shootingAudio.loop   = false;
        shootingAudio.volume = 1.0f;

        shootingAudio.Play();
        fired = true;
    }
}