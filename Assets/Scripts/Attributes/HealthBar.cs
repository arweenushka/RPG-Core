using UnityEngine;
using UnityEngine.UI;

namespace Attributes
{
    //one of the ways to do health bar with slider. used for player in the project
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;
        
        public void SetMaxHealth(float health)
        {
            slider.maxValue = health;
            slider.value = health;
        }

        public void SetHealth(float health)
        {
            slider.value = health;
        }
    }
}