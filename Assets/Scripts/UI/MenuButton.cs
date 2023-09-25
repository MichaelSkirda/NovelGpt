using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{

	private void Start()
	{
		Cursor.visible = true;
	}

	public void StartGameClick()
    {
        SceneManager.LoadScene("main");
	}

    public void ExitGameClick()
    {
        Application.Quit();
    }

}
