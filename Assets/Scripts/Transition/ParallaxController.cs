using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{

    [Header("PARALLAX")]
    public Transform parallaxBackground;
    public float parallaxScale;
    public float parallaxSmoothing;

    private Transform cameraTransform;
    private Vector3 cameraPosition;

    void Start()
    {
        findPrimaryCamera();
        findPositionCamera();
    }

    void LateUpdate()
    {
        float parallaxX = (cameraPosition.x - cameraTransform.position.x) * parallaxScale;
        float backgroundTargetX = parallaxBackground.position.x + parallaxX;

        Vector3 background = new Vector3(backgroundTargetX, parallaxBackground.localPosition.y, parallaxBackground.localPosition.z);
        parallaxBackground.position = Vector3.Lerp(parallaxBackground.position, background, parallaxSmoothing * Time.deltaTime);

        cameraPosition = cameraTransform.position;
    }

    void findPrimaryCamera()
    {
        cameraTransform = Camera.main.transform;
    }

    void findPositionCamera()
    {
        cameraPosition = cameraTransform.position;
    }
}
