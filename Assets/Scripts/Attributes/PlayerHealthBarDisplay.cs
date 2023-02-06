using System;
using TMPro;
using UnityEngine;

namespace Attributes
{
    //used if using healthBar instead of text statistic
    public class PlayerHealthBarDisplay : MonoBehaviour
    {
        private Health health;
        [SerializeField] private HealthBar healthBar;
        
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            //issue with set max health method. instead of 100 health  value is 111. defect opened
            //healthBar.SetMaxHealth(health.GetHealthPercentage());
            healthBar.SetHealth(health.GetHealthPercentage());
        }

        private void Update()
        {
            DisplayHealth();
        }
        
        private void DisplayHealth()
        {
            healthBar.SetHealth(health.GetHealthPercentage());
        }
    }
}
