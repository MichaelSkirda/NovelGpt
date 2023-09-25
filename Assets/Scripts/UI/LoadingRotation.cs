using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingRotation : MonoBehaviour
{
    public int AngelPerSecond;

	void Update()
    {
        gameObject.transform.eulerAngles += new Vector3(0, 0, AngelPerSecond * Time.deltaTime);
	}
}
