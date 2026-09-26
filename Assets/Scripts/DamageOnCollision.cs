using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    [SerializeField] private int damageValue = 10;
    [SerializeField] private string targetTag;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag(targetTag))
        {
            Health targetHealth = collision.gameObject.GetComponent<Health>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageValue);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageValue);
            }
        }
    }
}