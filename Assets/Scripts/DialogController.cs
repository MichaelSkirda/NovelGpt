using Assets.Scripts.Models;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{

    public TextMeshProUGUI TextField;
	public TextMeshProUGUI NameField;

	private DialogMessage Message { get; set; }
    private string DisplayText { get; set; }
	private string Text { get; set; }

    private IEnumerator Coroutine { get; set; }

    void Start()
    {
        ClearDialog();
	}
	void Update()
	{
		TextField.text = DisplayText;
	}

	public bool HasAnimationFinished()
	{
		if (Message == null)
			return true;
		if (Text == Message.Text)
			return true;
		return false;
	}

	public void SkipAnimation()
	{
		if (Coroutine != null)
			StopCoroutine(Coroutine);

		if(Message != null)
		{
			try
			{
				int drawedTextLength = Text.Length;
				string notDrawedText = Message.Text.Substring(drawedTextLength);

				DisplayText += notDrawedText;
				Text = Message.Text;
			}
			catch
			{
				Debug.LogError("CANNOT SKIP ANIMATION");
				DisplayText = Message.Text;
				Text = Message.Text;
			}
		}
	}

	public void ClearDialog()
	{
		DisplayText = string.Empty;
	}
	public void SetText(string text, string character = "")
	{
		TryStopCoroutine();
		DisplayText = text;
	}
	public void SetMessage(DialogMessage message)
    {
        Message = message;
	}
    
	public void DrawText(bool clear)
    {
		TryStopCoroutine();
		Text = "";

		if(clear)
			ClearDialog();

        Coroutine = AnimateText();
        StartCoroutine(Coroutine);
	}

	public void DrawText(DialogMessage message)
	{
		Message = message;
		DrawText(clear: true);
	}

	public void AppendText(DialogMessage message)
	{
		Message = message;
		DrawText(clear: false);
	}

	public void SetCharacterName(string name)
	{
		NameField.text = name;
	}

	private IEnumerator AnimateText()
    {
		string message = Message.Text;
		for(int i = 0; i < message.Length; i++)
        {
			char chr = message[i];

			// Draw tag without animation
			if(chr == '<')
			{
				DisplayText += chr;
				Text += chr;
				int attempt = 0;
				try
				{
					do
					{
						i++;
						attempt++;
						chr = message[i];
						DisplayText += chr;
						Text += chr;
						if (attempt > 15)
							throw new System.Exception("Too many atempts");
					} while (chr != '>');
					continue;
				}
				catch
				{
					Debug.LogError("CANNOT DRAW TAG!!!");
					SkipAnimation();
					yield break;
				}
			}

			// milliseconds to seconds
			float delay = Message.Delay / 1000f;
			yield return new WaitForSeconds(delay);

			DisplayText += chr;
			Text += chr;
		}
	}

	private void TryStopCoroutine()
	{
		if (Coroutine != null)
			StopCoroutine(Coroutine);
	}

}

