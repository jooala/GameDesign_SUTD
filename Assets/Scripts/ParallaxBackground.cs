using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float lenght;
    private float startPosition;
    public Transform mainCamera;
    public float parallaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;

    }

    // Update is called once per frame
    void Update()
    {
        float temp = (mainCamera.transform.position.x * (1 - parallaxEffect));
        float distance = (mainCamera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if (temp > startPosition + lenght) startPosition += lenght;
        else if (temp < startPosition - lenght) startPosition -= lenght;
    }
}
