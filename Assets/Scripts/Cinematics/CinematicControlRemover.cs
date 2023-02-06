using System;
using Controllers;
using Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private GameObject player;
        private void Awake() {
            player = GameObject.FindWithTag("Player");
        }

        private void OnEnable()
        {
            //events example. subscribe to some action. after scene was played/stopped call DisableControl or EnableControl
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }
        
        private void OnDisable() {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        //disable control of the player so he will stop all his current actions and couldn't start new before cinematic scene is finished
        private void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        //return control for the player
        private void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
