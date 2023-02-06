using Stats;
using TMPro;
using UnityEngine;

namespace Attributes
{
    public class PlayerExperienceDisplay : MonoBehaviour
    {
        private Experience experience;
        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            DisplayXP();
        }
        
        private void DisplayXP()
        {
            GetComponent<TextMeshProUGUI>().SetText("{0}",experience.GetExperience());
        }
    }
}