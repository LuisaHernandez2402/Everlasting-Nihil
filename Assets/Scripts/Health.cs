using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Character Type")]
    [SerializeField] private bool isPlayer = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"{gameObject.name} healed {healAmount}. Current HP: {currentHealth}");
    }

    private void Die()
    {
        if (isPlayer)
        {
            //Player
            Debug.Log("Player Died");
            gameObject.SetActive(false);
        }
        else
        {
            //Enemy
            Debug.Log($"{gameObject.name} was destroyed!");
            Destroy(gameObject);
        }
    }
}