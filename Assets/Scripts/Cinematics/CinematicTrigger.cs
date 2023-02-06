using UnityEngine;
using UnityEngine.Playables;

namespace Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private bool alreadyTriggered = false;

        //start the cinematic scene when player crossed specific  zone on the map
        private void OnTriggerEnter(Collider other) 
        {
           //add checks to check that scene is not played yet. to avoid multiply plays 
            if(!alreadyTriggered && other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}
