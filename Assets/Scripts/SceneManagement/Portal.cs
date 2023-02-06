using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    //used to trigger switching between scenes
    public class Portal : MonoBehaviour
    {
        //used to add the value to each portal to know in the future connection between portals.
        //ex. if you move from dest A from one scene you will appear on destination A as well on other scene
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }
        
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;

        //time for fading effect during switch between the scenes
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;


        //load specific scene when player trigger the portal object on the current scene
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }
        
        private IEnumerator Transition()
        {
            //to catch an error if scene is not set
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            //fade out effect when leaving the scene
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            
            //save level and its state
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            
            //scene loading
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            //load saved level and its state
            savingWrapper.Load();
            
            //yield return new WaitForEndOfFrame();
            
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            
            //sshould be added here for the 2nd time to load correctly previously saved state
            savingWrapper.Save();

            //fade in effect when entering the scene
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);
        }
        
        //put player in the spawn point of the portal and with rotation looking forward to the direction to go
        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            Vector3 directionVector = (otherPortal.spawnPoint.position - otherPortal.transform.position).normalized;
            player.transform.rotation = Quaternion.LookRotation(directionVector);

        }

        //find any other portal accept from which one you entered
        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}
