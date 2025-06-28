using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public enum Location { Farm, Home, Town, Ending}
    public Location currentLocation;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SwitchLocation(Location location)
    {
        SceneManager.LoadScene(location.ToString());
    }

    public void OnLocationLoad(Scene scene, LoadSceneMode mode)
    {
        Location oldLocation = currentLocation;

        Location newLocation = (Location)Enum.Parse(typeof(Location), scene.name);

        if(currentLocation == newLocation) { return; }

        currentLocation = newLocation;
    }
}
