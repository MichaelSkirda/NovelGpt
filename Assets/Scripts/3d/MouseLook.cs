using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float SensitivityVert = 6.0f;
    public float SensitivityHor = 6.0f;

    public float MinimumVert = -45.0f;
    public float MaximumVert = 45.0f;

    public float RotationX = 0;

	public TimerCountdown timer;

	[SerializeField]
    private Camera PlayerCamera;

    public bool IsPaused { get; set; } = true;

	void Start()
    {
        Cursor.visible = false;
        Rigidbody body = GetComponent<Rigidbody>();
        if(body != null)
        {
            body.freezeRotation = true;
        }
        var coroutine = UnpauseMouse(0.3f);
        StartCoroutine(coroutine);
    }

    private IEnumerator UnpauseMouse(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log("UNPAAAAAAAAAAAAAAAAAAAAAAAUSED!!!!!!");
        IsPaused = false;
        yield break;
    }

    
    void Update()
    {
        if (IsPaused)
            return;
        if(Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            timer.StartCountdown();
            Debug.Log("MOVEEED!!!");
        }

        RotationX -= Input.GetAxis("Mouse Y") * SensitivityVert;
        RotationX = Mathf.Clamp(RotationX, MinimumVert, MaximumVert);

        float delta = Input.GetAxis("Mouse X") * SensitivityHor;
        float rotationY = transform.localEulerAngles.y + delta;

        transform.localEulerAngles = new Vector3(0, rotationY, 0);
        PlayerCamera.transform.localEulerAngles = new Vector3(RotationX, 0, 0);
    }
}
