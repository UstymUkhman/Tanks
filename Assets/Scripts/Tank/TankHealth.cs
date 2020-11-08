using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public GameObject explosionPrefab;

    public float startingHealth  = 100f;

    public Color zeroHealthColor = Color.red;
    public Color fullHealthColor = Color.green;
    
    private bool dead;
    private float currentHealth;

    private AudioSource explosionAudio;
    private ParticleSystem explosionParticles;


    private void Awake()
    {
        explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio     = explosionParticles.GetComponent<AudioSource>();

        explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        dead          = false;
        currentHealth = startingHealth;

        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        SetHealthUI();

        if (currentHealth <= 0.0f && !dead)
        {
            OnDeath();
        }
    }

    private void SetHealthUI()
    {
        slider.value = currentHealth;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startingHealth);
    }

    private void OnDeath()
    {
        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);

        gameObject.SetActive(false);
        explosionParticles.Play();
        explosionAudio.Play();
        
        dead = true;
    }
}
