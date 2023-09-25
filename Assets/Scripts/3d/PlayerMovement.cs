using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody m_Rigidbody;
    public float m_Speed = 3f;

    public TimerCountdown timer;

    [SerializeField]
    private GameObject CrawlingCameraPoint;

    [SerializeField]
    private GameObject NormalCameraPoint;

    [SerializeField]
    private Camera PlayerCamera;
    public MouseLook MouseScript;

    private bool IsCrawling;

    private Vector3 m_Input;


    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
			MouseScript.IsPaused = false;
            timer.StartCountdown();
		}

		m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    void FixedUpdate()
    {

        float moveCoefficient = 1;

        if(IsCrawling && !Input.GetKey(KeyCode.LeftControl))
        {
            IsCrawling = false;
            PlayerCamera.transform.position = NormalCameraPoint.transform.position;     
        }
        

        Vector3 movement = new Vector3(m_Input.x, 0, m_Input.z);

        movement *= Time.deltaTime * m_Speed * moveCoefficient;
        movement = transform.TransformDirection(movement);


        m_Rigidbody.MovePosition(transform.position + movement);

        if (transform.localPosition.y <= -1000)
        {
			SceneManager.LoadScene("true_ending");
            return;
		}
	}

}