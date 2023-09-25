using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerCountdown : MonoBehaviour
{
	public TextMeshPro TextField;
	public GameObject Floor;
	public int TimeCount;
	public AudioSource Alarm;
	private bool IsSoundPlayed { get; set; } = false;

	public bool IsPaused { get; set; }
	private DateTime StartedAt { get; set; }

	private void Start()
	{
		IsPaused = true;
	}

	void Update()
    {
		if (IsPaused)
			return;

		TimeSpan delta = DateTime.Now - StartedAt;
		if(delta.TotalSeconds > TimeCount)
		{
			DeleteFloor();
			TextField.text = "00:00";
			return;
		}
		TimeSpan time = new TimeSpan(0, 0, seconds: TimeCount);
		TimeSpan timeLeft = time - delta;

		if(timeLeft.TotalSeconds <= 3 && !IsSoundPlayed)
		{
			IsSoundPlayed = true;
			Alarm.Play();
		}

		DrawText(timeLeft);
    }

	private void DrawText(TimeSpan timeLeft)
	{
		string format = @"ss\:ff";
		TextField.text = timeLeft.ToString(format);
	}

	public void StartCountdown()
	{
		// You can start only paused
		if (IsPaused == false)
			return;

		StartedAt = DateTime.Now;
		IsPaused = false;
	}

	private void DeleteFloor()
	{
		Floor.SetActive(false);
	}

}
