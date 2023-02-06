using System;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    //should be added to equipped weapon prefab 
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private UnityEvent onHit;
        public void OnHit()
        {
            onHit.Invoke();
        }
    }
}