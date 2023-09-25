using DAL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInputController : MonoBehaviour
{
    public RenSharpProcessor RenSharp;
    public TMP_InputField textInput;

    public string Context { get; set; }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            SendAnswer();
    }

    public void SendAnswer()
    {
        if(Context.ToLower() == "fakesafe")
        {
            string userInput = textInput.text;
			textInput.text = "";
			RenSharp.IsPaused = false;
			if (userInput != "0931")
			{
				RenSharp.GotoLabel("safeFail");
			}
            else
            {
				RenSharpProcessor.HasPassword = true;
			}
			RenSharp.ProccessCommandAfterPause();
			gameObject.SetActive(false);
            return;
		}

		DialogDto context = ParseContext(Context);
		RenSharp.GetAnswer(context, textInput.text, Context);
        textInput.text = "";
		gameObject.SetActive(false);
	}

    public DialogDto ParseContext(string location)
    {
        if(location == "zone1")
        {
            return new DialogDto()
            {
                Messages = new List<MessageDto>()
            };
        }

		return new DialogDto()
		{
            Messages = new List<MessageDto>()
            { 
                new MessageDto()
                {
                    IsPlayerMessage = false,
                    Text = " ŒÕ“≈ —“ Õ≈ ¡€À Õ¿…ƒ≈Õ!!!"
                }
            },
            Location = location
		};
	}

}
