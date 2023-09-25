using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameButtons : MonoBehaviour
{

    public RenSharpProcessor RenSharp;
    public bool WasPaused { get; set; } = false;

    public void ContinueClick()
    {
        if(WasPaused)
        {
			RenSharp.IsPaused = true;
		}
        else
        {
			RenSharp.IsPaused = false;
		}
		gameObject.SetActive(false);
    }

    public void GotoMainMenu()
    {
		SceneManager.LoadScene("mainmenu");
        return;
	}

}
