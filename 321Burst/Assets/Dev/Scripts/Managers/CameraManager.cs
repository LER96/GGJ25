using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public GameObject CameraShakeObj;

    private float timer = 0f;
    private bool isShaking = false; // Flag to track if the shake is active

    private void Awake()
    {
        // Ensure singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isShaking)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                CameraShakeObj.SetActive(false); // Deactivate the shake
                isShaking = false; // Reset the flag
            }
        }
    }

    public void Shake(float duration)
    {
        CameraShakeObj.SetActive(true); // Activate the shake
        timer = duration; // Reset the timer
        isShaking = true; // Set the shaking flag
    }
}
