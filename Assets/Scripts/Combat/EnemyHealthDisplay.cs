using Attributes;
using TMPro;
using UnityEngine;

namespace Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;
        private Health targetHealth;
        
        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        
        //TODO update with delegate. get target only if choosed.not check every frame
        private void Update()
        {
            targetHealth = fighter.GetTarget();
            DisplayHealth();
        }
        
        //when player has no target display N/A health
        private void DisplayHealth()
        {
            if (targetHealth == null || targetHealth.IsDead())
            {
                GetComponent<TextMeshProUGUI>().SetText("N/A");
            }
            else
            {
                //show health with 1 decimal
               // GetComponent<TextMeshProUGUI>().SetText("{0:0}%", targetHealth.GetHealthPercentage());
                GetComponent<TextMeshProUGUI>().SetText("{0:0}/{1:0}", targetHealth.GetHealthPoints(), targetHealth.GetMaxHealthPoints());
            }
        }
    }
}
