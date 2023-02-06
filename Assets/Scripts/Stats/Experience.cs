using System;
using Saving;
using UnityEngine;

namespace Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoints = 0;

        //delegate with void type could be changed to Action
        public event Action onExperienceGained; 

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperience()
        {
            return experiencePoints;
        }
        
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}