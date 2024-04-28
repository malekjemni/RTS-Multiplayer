using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class CameraMouvment : NetworkBehaviour
{

    [SerializeField] private CinemachineFreeLook freeLookCameraToZoom; 
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            freeLookCameraToZoom = CinemachineFreeLook.FindObjectOfType<CinemachineFreeLook>();
            freeLookCameraToZoom.LookAt = this.gameObject.transform;
            freeLookCameraToZoom.Follow = this.gameObject.transform; 

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
