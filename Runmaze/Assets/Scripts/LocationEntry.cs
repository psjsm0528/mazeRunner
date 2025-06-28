using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationEntry : MonoBehaviour
{

    [SerializeField] SceneTransitionManager.Location location;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneTransitionManager.Instance.SwitchLocation(location);
        }
    }

}
