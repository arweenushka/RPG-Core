using System;
using TMPro;
using UnityEngine;

namespace Attributes
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        private Health health;
        
        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            DisplayHealth();
        }
        
        private void DisplayHealth()
        {
            //show health with 1 decimal
            //GetComponent<TextMeshProUGUI>().SetText("{0:0}%", health.GetHealthPercentage());
            GetComponent<TextMeshProUGUI>().SetText("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
