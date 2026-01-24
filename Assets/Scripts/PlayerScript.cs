using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; // привязываем сюда созданный нами Image типа Slider
    [SerializeField] private Text healthText;     // привязываем сюда созданный нами Text

    private float maxHealth = 100f;   // максимальное количество здоровья
    private float currentHealth;      // текущее здоровье

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            //  механика смерти игрока

        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        healthSlider.value = Mathf.Clamp(currentHealth / maxHealth, 0f, 1f); // обновляем индикатор
        healthText.text = $"HP: {currentHealth:F0}/{maxHealth}";
    }
}
