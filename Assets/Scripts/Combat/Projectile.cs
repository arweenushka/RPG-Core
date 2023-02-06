using Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float arrowSpeed = 1f;
        private Health target = null;
        private float damage = 0;
        //chosen weapon will be attacker that used fot getting experience points for player 
        GameObject attackInitiator = null;

        [SerializeField] private GameObject hitEffect = null;
        //put as true this checkbox on projectile in editor which should follow the target during the shooting
        [SerializeField] private bool isFollowingTarget = true;
        [SerializeField] private float maxLifeTime = 10;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 2;

        [SerializeField] private UnityEvent onHit;

        // Start is called before the first frame update
        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if (isFollowingTarget && !target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * (arrowSpeed * Time.deltaTime));
        }

        public void SetTarget(Health target, GameObject attackInitiator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.attackInitiator = attackInitiator;
            //to destroy projectile that are flying of the scene(if shoot when enemy is dead for example) after some period of time
            Destroy(gameObject, maxLifeTime);
        }

        //get location of the body of the target to shoot arrow to that place
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            return targetCapsule.Equals(null)
                ? target.transform.position
                : target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            //check first that object that collides with projectile contain health component
            if (other.gameObject.GetComponent<Health>() != target) return;
            //to do not shoot to empty collider when target is dead already but collider is on the scene
            if (target.IsDead()) return;
            target.TakeDamage(attackInitiator, damage);
            
            //play hit sound
            onHit.Invoke();
            
            arrowSpeed = 0;
            //play VFX effect of projectile if exist
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            
            //used to destroy different parts of projectile object in different time. first head then effect trail for example
            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            Destroy(gameObject, lifeAfterImpact);
        }

    }
}