using UnityEngine;

namespace Core
{
    public class Destroyer: MonoBehaviour
    {
        [SerializeField] private GameObject targetToDestroy = null;

        //used in animation unity event in DamageTextSpawner to destroy damage text object after showing it
        public void DestroyTarget()
        {
            Destroy(targetToDestroy);
        }
    }
}