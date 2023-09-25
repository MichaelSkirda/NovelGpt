using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MusicFade
{
    public static IEnumerator FadeOut(AudioSource audio, float fadeTime)
    {
        if (fadeTime == 0)
            fadeTime = 0.1f;

        float startVolume = audio.volume;

        while(audio.volume > 0)
        {
            audio.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audio.Pause();
    }

    public static IEnumerator FadeIn(AudioSource audio, float fadeTime, float targetVolume)
    {
		audio.UnPause();

		while (audio.volume < targetVolume)
		{
			audio.volume += 1f * Time.deltaTime / fadeTime;
			yield return null;
		}

        audio.volume = 1f;
	}
}
