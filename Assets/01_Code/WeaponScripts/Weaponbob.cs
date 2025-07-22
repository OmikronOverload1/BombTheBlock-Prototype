using Unity.VisualScripting;
using UnityEngine;

public class Weaponbob : MonoBehaviour
{
    [Range(0.0001f, 1f)]
    public float headBobAmount = 0.0001f;

    [Range(0.1f, 1000f)]
    public float headBobFrequency = 10f;

    [Range(0.1f, 100f)]
    public float headBobSmooth = 10f;

    private float crouchMultiplier = 2f;
    private float sprintMultiplier = 1.5f;

    

    [Header("Runtime Filled")]
    [SerializeField] private InputHelper inputHelper;
    [SerializeField] private PlayerController playerController;

    Vector3 StartPos;


    private void Start()
    {
        StartPos = transform.localPosition;
    }

    private void Update()
    {
        HeadbobState();
        StopHeadbob();
    }

    private void HeadbobState()
    {
        float inputMagnitude = new Vector3(inputHelper.MoveInput.x, 0f, inputHelper.MoveInput.y).magnitude;

        if (inputMagnitude > 0)
        {
            float multiplier = 1f;
            if (inputHelper.IsCrouching && inputMagnitude > 0) multiplier = multiplier / crouchMultiplier;

            else if (inputHelper.IsSprinting && inputMagnitude > 0) multiplier = multiplier + sprintMultiplier;

            StopHeadbob();
            StartHeadbob(multiplier);
        }
            else
        {
            StopHeadbob();
        }
    }

    private Vector3 StartHeadbob(float multiplier)
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Lerp(0f, Mathf.Cos(Time.time * headBobFrequency) * headBobAmount * multiplier * 1.5f, headBobSmooth * Time.deltaTime);
        pos.x = Mathf.Lerp(0f, Mathf.Sin(Time.time * headBobFrequency / 2f) * headBobAmount * multiplier * 1.6f, headBobSmooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }

    private void StopHeadbob()
    {
        if (transform.localPosition == StartPos)
            return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, headBobSmooth * Time.deltaTime);
    }
}