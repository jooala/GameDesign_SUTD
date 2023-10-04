using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public GameObject flashlightLight; // Reference to the flashlight's Light component.
    public bool toggleFlashlight = false; // Set to true to enable flashlight toggling.

    private bool isFlashlightOn = true;
    private float toggleInterval = 5f;
    private float flashDuration = 0.2f;
    private float toggleTimer = 0f;
    private bool isFlashing = false;
    private int flashCount = 0;

    private void Update()
    {
        if (toggleFlashlight)
        {
            toggleTimer += Time.deltaTime;

            if (!isFlashing && toggleTimer >= toggleInterval)
            {
                // Start flashlight toggle.
                isFlashing = true;
                toggleTimer = 0f;
                ToggleLight();
            }
            else if (isFlashing && toggleTimer >= flashDuration)
            {
                // Toggle flashlight on/off for short durations.
                flashCount++;

                if (flashCount <= 2)
                {
                    ToggleLight();
                    toggleTimer = 0f;
                }
                else
                {
                    // End flashlight toggle after 2 flashes.
                    isFlashing = false;
                    flashCount = 0;
                }
            }
        }
        else if (!toggleFlashlight && !isFlashlightOn)
        {
            // Ensure the flashlight is turned off when not toggling.
            flashlightLight.SetActive(false);
            isFlashlightOn = false;
        }
    }

    private void ToggleLight()
    {
        isFlashlightOn = !isFlashlightOn;
        flashlightLight.SetActive(isFlashlightOn);
    }
}
