using DAL.Models.Gpt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RenSharpServer.Services
{
	public class GptService : MonoBehaviour
	{

		private string Key { get; set; } = "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";
		private string Url { get; set; } = "https://api.openai.com/v1/chat/completions";
		private string GptModel { get; set; } = "gpt-3.5-turbo";

		private string Prompt { get; set; } =
@"Ты программа, которая анализирует ввод пользователя. Ты должна сопоставить его с наиболее подходящим к ситуации вариантом. Пиши только самый подходящий вариант из списка и ничего более. 

Ситуация: ты очутился в неизвестной комнате. Рядом с тобой кто-то другой. Ты не знаешь как сюда попал и где ты.

Варианты:
1. ""Кто ты?""
2. ""Как тебя зовут?""
3. ""Как ты сюда попал?""
4. ""Как я сюда попал?""
5. ""Ты меня знаешь?""
6. ""Сколько тебе лет?""
7. ""Где мы?""
8. ""Как мы здесь оказались?""
9. ""Нас похители?""
10. ""Что здесь происходит?""
11. ""Что случилось?""
12. ""Что ты тут делаешь?""
13. ""Как довно мы тут?""
14. ""Мы находимся в игре.""
15. ""Угроза""
16. ""Ты искусственный интеллект?""
17. ""Как выбраться отсюда?""
18. ""Ты работаешь на корпорацию?""
19. ""Ты знаешь где мой отец?""
20. ""Ты работаешь на сопротивление""
21. ""Ты знаешь о сопротивлении?""
22. ""Что ты знаешь про корпорацию?""
23. ""У тебя есть оружие?""
24 ""У тебя сняли чип?""
25. ""Кем ты работаешь?""
26. ""У тебя есть семья?""
27. ""Нет варианта""";


		public IEnumerator MapAnswer(string input, Action<string> callback)
		{
			var messagePrompt = new Message() { role = "system", content = Prompt };
			var message = new Message() { role = "user", content = input };

			List<Message> messages = new List<Message> { messagePrompt, message };

			// формируем отправляемые данные
			var requestData = new Request()
			{
				model = GptModel,
				messages = messages,
				temperature = 0.15f
			};

			string json = JsonUtility.ToJson(requestData);
			Debug.Log("Self gpt json: " + json);

			var request = UnityWebRequest.Put(Url, json);
			request.method = "POST";
			request.SetRequestHeader("Authorization", Key);
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("SELF-GPT error response: " + request.error);
				Debug.Log("SELF-GPT status code: " + request.responseCode);
				callback(null);
				yield break;
			}

			string responseJson = request.downloadHandler.text;
			ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);

			var choices = responseData?.choices ?? new List<Choice>();
			if (choices.Count == 0)
			{
				Debug.Log("No gpt answer");
			}
			var choice = choices[0];
			var responseMessage = choice.message;
			// добавляем полученное сообщение в список сообщений
			messages.Add(responseMessage);
			var responseText = responseMessage.content.Trim();

			Debug.Log("Self gpt answer: " + responseText);
			callback(responseText);
			yield break;
		}
	}
}
