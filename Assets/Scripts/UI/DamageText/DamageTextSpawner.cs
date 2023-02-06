using System;
using Attributes;
using TMPro;
using UnityEngine;

namespace UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab;

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            //set value to text component
            instance.SetValue(damageAmount);

        }
    }
}