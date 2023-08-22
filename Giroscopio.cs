using System.Collections;
using UnityEngine;

public class Gyroscopio : MonoBehaviour
{
    private float initialYAngle = 0f;
    private float appliedGyroYAngle = 0f;
    private float calibrationYAngle = 0f;
    private Transform rawGyroRotation;
    private float tempSmoothing;

    [SerializeField] private float smoothing = 0.1f;

    private void Update()
    {
        ApplyGyroRotation();
        ApplyCalibration();
        float rotationSpeed = 2f;
        transform.rotation = Quaternion.Slerp(transform.rotation, rawGyroRotation.rotation, Time.deltaTime * rotationSpeed);
    }

    private IEnumerator Start()
    {
        Input.gyro.enabled = true;
        Application.targetFrameRate = 60;
        initialYAngle = transform.eulerAngles.y;

        rawGyroRotation = new GameObject("GyroRaw").transform;
        rawGyroRotation.position = transform.position;
        rawGyroRotation.rotation = transform.rotation;
        yield return new WaitForSeconds(1f);
        StartCoroutine(CalibrateYAngle());
    }

    private IEnumerator CalibrateYAngle()
    {
        tempSmoothing = smoothing;
        smoothing = 1f;
        calibrationYAngle = appliedGyroYAngle - initialYAngle;
        yield return null;
        smoothing = tempSmoothing;
    }

    private void ApplyGyroRotation()
    {
        rawGyroRotation.rotation = Input.gyro.attitude;
        rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self);
        rawGyroRotation.Rotate(90f, 180f, 0f, Space.World);
        appliedGyroYAngle = rawGyroRotation.eulerAngles.y;
    }

    private void ApplyCalibration()
    {
        rawGyroRotation.Rotate(0f, -calibrationYAngle, 0f, Space.World);
    }

    public void SetEnabled(bool value)
    {
        enabled = true;
        StartCoroutine(CalibrateYAngle());
    }
}
