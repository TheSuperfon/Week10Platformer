using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController2D : MonoBehaviour
{
    [SerializeField] private Camera followCamera;
    private Vector2 viewportHalfSize;
    private float leftBoundryLimit;
    private float rightBoundryLimit;
    private float bottomBoundryLimit;

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float smoothing = 5f;

    private Vector3 shakeOffset = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        tilemap.CompressBounds();
        CalculateCameraBoundries();
    }

    private void CalculateCameraBoundries()
    {
        viewportHalfSize = new Vector2(followCamera.orthographicSize * followCamera.aspect, followCamera.orthographicSize);

        leftBoundryLimit = tilemap.transform.position.x + tilemap.cellBounds.min.x + viewportHalfSize.x;

        rightBoundryLimit = tilemap.transform.position.x + tilemap.cellBounds.max.x - viewportHalfSize.x;

        bottomBoundryLimit = tilemap.transform.position.y + tilemap.cellBounds.min.y + viewportHalfSize.y;

    }


    // Update is called once per frame
    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Shake(2.5f, 3f);
        }

        Vector3 desiredPosition = target.position + new Vector3(offset.x, offset.y, transform.position.z) + shakeOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 - Mathf.Exp(-smoothing * Time.deltaTime));

        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, leftBoundryLimit, rightBoundryLimit);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, bottomBoundryLimit, smoothedPosition.y);

        transform.position = smoothedPosition;

    }


    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }


    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shakeOffset = Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;

    }

}
