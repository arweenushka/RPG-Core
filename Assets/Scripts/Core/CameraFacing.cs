using UnityEngine;

namespace Core
{
    //another way to use Look At in Billboard script
    public class CameraFacing : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}