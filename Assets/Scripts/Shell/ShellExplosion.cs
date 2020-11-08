using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public float maxLifeTime     = 2.0f;
    public float explosionRadius = 5.0f;
    public float maxDamage       = 100.0f;
    public float explosionForce  = 1000.0f;

    public LayerMask tankMask;
    public AudioSource explosionAudio;
    public ParticleSystem explosionParticles;


    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody target = colliders[i].GetComponent<Rigidbody>();

            if (target)
            {
                target.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                TankHealth targetHealth = target.GetComponent<TankHealth>();

                if (targetHealth)
                {
                    float damage = CalculateDamage(target.position);
                    targetHealth.TakeDamage(damage);
                }
            }
        }

        explosionParticles.transform.parent = null;
        explosionParticles.Play();
        explosionAudio.Play();

        Destroy(explosionParticles.gameObject, explosionParticles.main.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 target = targetPosition - transform.position;
        float distance = (explosionRadius - target.magnitude) / explosionRadius;

        return Mathf.Max(0.0f, distance * maxDamage);
    }
}
