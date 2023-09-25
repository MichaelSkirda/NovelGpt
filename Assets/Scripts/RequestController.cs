using DAL.Models;
using RenSharpServer.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RequestController : MonoBehaviour
{
	public GptService GptService;

	public bool IsServerUp { get; set; }
	private static string ServerUrl { get; set; } = "http://194.58.123.154/";
	private static string HandshakeEndpoint { get; set; } = "server/status";
	private static string ParseMessageEndpoint { get; set; } = "dialog/startparsing";
	private static string DialogStatusEndpoint { get; set; } = "dialog/getstatus/";
	private static string GptParseEndpoint { get; set; } = "dialog/useGpt/";
	private static string SetAcceptedEndpoint { get; set; } = "dialog/accept";
	private static string ServerModeEndpoint { get; set; } = "server/getmode";

	void Start()
    {
		StartCoroutine(ServerHandshake((x) =>
		{
			if (x == null)
			{
				IsServerUp = false;
			}
			else
			{
				x = x.ToLower().Trim();
				if (x.StartsWith("ok"))
					IsServerUp = true;
			}

			if (IsServerUp)
				Debug.Log("Server is OK!");
			else
				Debug.LogError("Server is shutdown");
		}));

		StartCoroutine(GetServerMode(isManual =>
		{
			if (isManual)
				Debug.Log("Server is manual!");
			else
				Debug.Log("Server is not manual!");
		}));
	}

	public IEnumerator ServerStartParsing(DialogDto dialog, Action<int> response)
	{
		string url = ServerUrl + ParseMessageEndpoint;
		string json = JsonUtility.ToJson(dialog);
		using var request = UnityWebRequest.Put(url, json);
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("Accept", "text/csv");
		request.method = "POST";
		request.timeout = 10;

		yield return request.SendWebRequest();



		if (request.result == UnityWebRequest.Result.Success)
		{
			int result;
			try
			{
				result = Int32.Parse(request.downloadHandler.text);
			}
			catch
			{
				// use GPT
				Debug.LogError("Server returned not int ID");
				response(-1);
				yield break;
			}
			response(result);
		}
		else
		{
			response(-1);
			Debug.LogError("Parsing error response: " + request.error + " | Status code: " + request.responseCode);
			yield break;
		}

		yield break;
	}

	public IEnumerator GetResponseFromServer(int id, string message, Action<string> callback)
	{
		string response = null;
		yield return StartServerPolling(id, (x) => response = x);

		if(IsAnswerValid(response))
		{
			callback(response);
			StartCoroutine(SetAccepted(id, response));
			yield break;
		}

		callback(null);
		StartCoroutine(SetAccepted(id, "$SKIP"));
		yield break;

		
		/*
		yield return GetGptAnswer(id, x => response = x);
		if(IsAnswerValid(response))
		{
			callback(response);
			StartCoroutine(SetAccepted(id, response));
			yield break;
		}

		yield return GptService.MapAnswer(message, x => response = x);

		if(IsAnswerValid(response))
		{
			callback(response);
			yield break;
		}

		callback(null);
		yield break; */
	}

	private IEnumerator StartServerPolling(int id, Action<string> callback)
	{
		string url = ServerUrl + DialogStatusEndpoint + id;
		var pollingStarted = DateTime.Now;
		int timeout = 10;

		string response;

		while (true)
		{
			DateTime startedAt = DateTime.Now;
			using var request = UnityWebRequest.Get(url);
			request.timeout = 20;
			yield return request.SendWebRequest();
			yield return new WaitForSecondsRealtime(1);

			response = request.downloadHandler.text;
			response = response.Replace("\n", "").Trim().Trim('"').Trim();
			if (response.ToLower() == "$wait_more" || response.ToLower() == "\"$wait_more\"")
			{
				timeout = 60;
			}

			var now = DateTime.Now;
			var delta = now - startedAt;
			var expires = pollingStarted.AddSeconds(timeout);
			Debug.Log($"Poll {id} finished. DeltaTime: {delta.TotalMilliseconds}ms. | Time left: {(expires - now).TotalMilliseconds}ms.");

			if (IsAnswerValid(response))
			{
				callback(response);
				yield break;
			}

			if (now > expires)
			{
				callback(response);
				Debug.Log("StartServerPolling TIME OUT!");
				yield break;
			}

		}
	}

	private bool IsAnswerValid(string message)
	{
		message = message.ToLower();
		if (message != "" && message != null && message != "$wait_more" && message != "\"$wait_more\"" && message != "null" && message != "wait_more" && message != "\"wait_more\"")
			return true;
		return false;
	}

	IEnumerator SetAccepted(int dialogId, string message)
	{
		string url = ServerUrl + SetAcceptedEndpoint;
		var pair = new IdMessagePairDto() { DialogId = dialogId, Message = message };
		string json = JsonUtility.ToJson(pair);

		using var request = UnityWebRequest.Put(url, json);
		request.SetRequestHeader("Content-Type", "application/json");
		request.SetRequestHeader("Accept", "text/csv");
		request.method = "POST";

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success)
			Debug.LogError("Parsing error response: " + request.error);
		yield break;
	}

	IEnumerator GetGptAnswer(int dialogId, Action<string> callback)
	{
		string url = ServerUrl + GptParseEndpoint + dialogId;
		using var request = UnityWebRequest.Get(url);
		request.timeout = 20;

		yield return request.SendWebRequest();

		if(request.result == UnityWebRequest.Result.Success)
			callback(request.downloadHandler.text);
		else
		{
			callback(null);
		}

		yield break;
	}


	IEnumerator ServerHandshake(Action<string> response)
	{
		string url = ServerUrl + HandshakeEndpoint;
		using var request = UnityWebRequest.Get(url);

		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			response(request.downloadHandler.text);
		}
		else
		{
			Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
			response(null);
		}

		request.Dispose();
	}

	public IEnumerator GetServerMode(Action<bool> callback)
	{
		string url = ServerUrl + ServerModeEndpoint;
		using var request = UnityWebRequest.Get(url);
		request.timeout = 2;

		yield return request.SendWebRequest();

		if(request.result == UnityWebRequest.Result.Success)
		{
			string mode = request.downloadHandler.text;
			mode = mode.ToLower().Trim();
			if(mode == "true" || mode == "1")
			{
				callback(true);
			}
			else
			{
				callback(false);
			}
		}
		else
		{
			callback(false);
		}
		yield break;
	}

}
