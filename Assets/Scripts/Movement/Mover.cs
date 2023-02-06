using System;
using Attributes;
using Core;
using Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Movement
{
    //means that script will not work if there is no required component. in this case nav mesh agent
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private float distanceToStop;
        private ActionScheduler actionScheduler;
        private Health health;

        //max speed of character
        [SerializeField] private float maxSpeed = 5f;
        //distance that is allowed for the player to go with one click 
        [SerializeField] float maxNavPathLength = 40f;

        private void Awake()
        {
            health = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Update is called once per frame
        private void Update()
        {
            //nav mesh agent of the object is enabled only if the object is not dead
            //(used to avoid movement problems for object that is going on the way with dead body)
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        //move to some point with speed. speed depends of the character.
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //stop previous action and start new one
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }
        
        // method to move to the specific point on the scene using raycast
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            //speed will depends of movement type(patrol or attack)
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            //move to only if Stop method is not called
            navMeshAgent.isStopped = false;
        }
        
        //stop previous action of the object( from interface)
        public void CancelAction()
        {
            navMeshAgent.isStopped = true;
        }

        //update animator component with certain speed. take speed value in nav mesh agent and send to animator component(forwardSpeed field)
        private void UpdateAnimator()
        {
            //get global velocity and transform it to local velocity for the animation
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            //speed that we used in animator
            animator.SetFloat("forwardSpeed", speed);
        }

        //additional structure to be able to add different elements/component to save file. in our case position and rotation
        [Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }
        
        
        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }
                
        //used to do not allow to the player go to far after cursor pointing. if distance is too far away, then playe can go there
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        //method inherited from ISaveble interface that is used for Saving statements in the game.(downloaded from the Saving package)
        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            //position and rotation will be added to the save file 
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }
        
        //should be called always after awake but before start
        //method inherited from ISaveble interface that is used for Saving statements in the game.(downloaded from the Saving package)
        public void RestoreState(object state)
        { 
            //load position and rotation from previously saved state
           MoverSaveData data = (MoverSaveData) state;
           navMeshAgent.enabled = false;
           transform.position = data.position.ToVector();
           transform.eulerAngles = data.rotation.ToVector();
           navMeshAgent.enabled = true;
           
            //Wrap used for the same reason like enabled false and the true.was using wheen we were saving only position          
           /*SerializableVector3 position = (SerializableVector3) state;
           GetComponent<NavMeshAgent>().Warp(position.ToVector());*/
        }
    }
}