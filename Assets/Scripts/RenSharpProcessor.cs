using Assets.Scripts;
using Assets.Scripts.Models;
using DAL.Models;
using RenSharp;
using RenSharp.Models;
using RenSharp.Models.CommandAttributes;
using RenSharpServer.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RenSharpProcessor : MonoBehaviour
{

    public DialogController Dialog;
    [SerializeField]
    public TextAsset RenSharpCode;

	public Image Background;
	public Sprite BarSprite;
    public Sprite Surgery;
    public Sprite Reception;
    public Sprite Elevator;
    public Sprite PreCompany;
    public Sprite LeaderOffice1;
    public Sprite LeaderOffice2;
    public Sprite LeaderOffice3;
    public Sprite LeaderOffice4;

	public Image Character;
    public Image Character2;
	public Sprite AlexSprite;
    public Sprite SkivFace;
    public Sprite MC_Sitting;
    public Sprite JohnSprite;
    public Sprite SaraSprite;
    public Sprite ZeronSprite;

	public TextInputController TextInputController;
    public RequestController RequestController;
    public ButtonsContoller ButtonsContoller;
    public GameObject LoadingIndicator;

    public AudioSource Music;
    public AudioSource SoundEffect;

    public AudioClip AlarmReception;
    public AudioClip PhoneRing;
    public AudioClip ShotSound;

    public GameButtons MainMenu;

    public float AudioFadeTime;

	public bool IsPaused { get; set; }
    public static bool HasPassword { get; set; } = false;

    private GameManager RenSharp { get; set; }
    private DialogWriter Writer { get; set; }

    private List<CommandType> SkipCommand = new List<CommandType>()
    {
        CommandType.ChangeBG,
        CommandType.Goto,
        CommandType.ChangeCharacter,
        CommandType.ChangeCharacter2,
        CommandType.HideCharacter,
        CommandType.HideCharacter2,
        CommandType.Label,
        CommandType.Nop,
        //CommandType.EndOfGame,
        CommandType.RealEndOfGame,
        CommandType.EndOfSubprogram,
        CommandType.PlaySound
    };

	void Start()
    {
		Application.runInBackground = true;

        Character.gameObject.SetActive(false);
        Character2.gameObject.SetActive(false);
		string[] lines = RenSharpCode.text.Split('\n');
		Writer = new DialogWriter(Dialog);
        RenSharp = new GameManager(lines, Writer);

        if(HasPassword)
        {
            GotoLabel("hasPassword");
        }

        ProccessCommandAfterPause();
        //string filepath = Application.dataPath + "/fileExample.csren";
        //RenSharp = new GameManager(filepath, Writer);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {
			Dialog.SkipAnimation();
			MainMenu.WasPaused = IsPaused;
			IsPaused = true;
            MainMenu.gameObject.SetActive(true);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) == false && Input.GetKeyDown(KeyCode.Space) == false)
            return;

        if(IsPaused)
        {
            Debug.Log("Game is paused!!");
            return;
        }

        if(Dialog.HasAnimationFinished() == false)
        {
            Dialog.SkipAnimation();
            return;
        }

        Command command;
		do
        {
            command = RenSharp.ReadNextLine();
            Debug.Log($"Command type: {command.Type} at line {command.Line} value {command.Value}");
			ProcessCommand(command);
		} while (SkipCommand.Contains(command.Type) && IsPaused == false);


    }

    private void ProcessCommand(Command command)
    {
        if (command == null)
        {
            Debug.LogError("COMMAND WAS NULL!");
			return;
		}

		if (command.Type == CommandType.EndOfGame)
		{
            HandleEndOfGame(command.Value);
		}
		else if (command.Type == CommandType.ChangeBG)
		{
            string background = command.Value.ToLower();
            ChangeBG(background);
		}
        else if(command.Type == CommandType.ChangeCharacter)
        {
            int posX = command.Attributes.GetPosX();
            Vector3 position = new Vector3(posX, Character.transform.localPosition.y, Character.transform.localPosition.z);
            Character.transform.localPosition = position;
			Character.gameObject.SetActive(true);
			string characterName = command.Value.ToLower();
            ChangeCharacter(characterName);
        }
        else if(command.Type == CommandType.ChangeCharacter2)
        {
			int posX = command.Attributes.GetPosX();
			Vector3 position = new Vector3(posX, Character2.transform.localPosition.y, Character2.transform.localPosition.z);
			Character2.transform.localPosition = position;
			Character2.gameObject.SetActive(true);
			string characterName = command.Value.ToLower();
			ChangeCharacter2(characterName);
		}
        else if(command.Type == CommandType.HideCharacter)
        {
            Character.gameObject.SetActive(false);
        }
        else if(command.Type == CommandType.HideCharacter2)
        {
            Character2.gameObject.SetActive(false);
        }
        else if(command.Type == CommandType.InternetCheck)
        {
            HandleInternetCheck(command.Value);
        }
        else if(command.Type == CommandType.Textinput)
        {
            IsPaused = true;
            var coroutine = CheckServerConnection(command.Value);
            StartCoroutine(coroutine);
        }
        else if(command.Type == CommandType.PlaySound)
        {
            if(command.Value == "alarmReception")
            {
				SoundEffect.clip = AlarmReception;
				SoundEffect.Play();
			}  
            else if(command.Value == "phoneRing")
            {
                SoundEffect.clip = PhoneRing;
                SoundEffect.Play();
            }
            else if(command.Value == "shot")
            {
                SoundEffect.clip = ShotSound;
                SoundEffect.Play();
            }
            else if(command.Value == "stopMusic")
            {
                var coroutine = MusicFade.FadeOut(Music, AudioFadeTime);
                StartCoroutine(coroutine);
            }
            else if(command.Value == "startMusic")
            {
                var coroutine = MusicFade.FadeIn(Music, AudioFadeTime, targetVolume: 1f);
				StartCoroutine(coroutine);
			}
            else
            {
                Debug.LogError("NO SOUND WAS FOUND");
            }
        }
        else if(command.Type == CommandType.Choice)
        {
            IsPaused = true;
            ButtonsContoller.EnableButton(command.Value);
        }
        else if(command.Type == CommandType.RealEndOfGame)
        {
            IsPaused = false;
            GotoLabel("theEnd");
        }
	}

    private void HandleInternetCheck(string value)
    {
        IsPaused = true;
        if(value == null || value == "")
        {
            Debug.LogError("INTERNET CHECK NULL OR EMPTY!!!");
            IsPaused = false;
			return;
		}

		if (value == "afterOperationCheck1")
        {
            LoadingIndicator.SetActive(true);
            var coroutine = RequestController.GetServerMode(hasInternet => HandleInternetCheck(value, hasInternet));
            StartCoroutine(coroutine);
        }
        else
        {
            Debug.LogError($"LABEL {value} NOT FOUND!!!");
            IsPaused = false;
            return;
		}
    }

    private void HandleInternetCheck(string value, bool hasInternet)
    {
        Debug.Log("HAS INTERNET: " + hasInternet);
        LoadingIndicator.SetActive(false);
        IsPaused = false;
		if (hasInternet == false)
        {
            GotoLabel(value + "_offline");
			ProccessCommandAfterPause();
			return;
        }
		ProccessCommandAfterPause();
	}


	private void HandleEndOfGame(string ending)
    {
        IsPaused = false;
        if (ending == null || ending == "" || ending == "theEnd")
        {
            GotoLabel("theEnd");
        }
        else if (ending == "menu")
        {
			SceneManager.LoadScene("mainmenu");
            return;
		}
        else if(ending == "goto3d")
        {
			SceneManager.LoadScene("3dscene");
            return;
		}
        else if(ending == "true_ending")
        {
			SceneManager.LoadScene("true_ending");
			return;
	    }


	}

	private IEnumerator CheckServerConnection(string location)
	{
        if(location == "fakeSafe")
        {
			TextInputController.Context = "fakeSafe";
			TextInputController.gameObject.SetActive(true);
			yield break;
        }    
        bool hasInternet = false;
        yield return RequestController.GetServerMode(x => hasInternet = x);

        if(hasInternet)
        {
            TextInputController.Context = location;
            TextInputController.gameObject.SetActive(true);
            yield break;
        }

        GotoLabel(location + "_offline");
        IsPaused = false;
        yield break;
	}


	public void ProccessCommandAfterPause()
    {
		Command command;
		do
		{
			command = RenSharp.ReadNextLine();
			Debug.Log($"[AFTER PAUSE] Command type: {command.Type} at line {command.Line}");
			ProcessCommand(command);
		} while (SkipCommand.Contains(command.Type) && IsPaused == false);

        if(command.Type != CommandType.Choice && command.Type != CommandType.Textinput)
            IsPaused = false;
	}

    public void GotoLabel(string label)
    {
        RenSharp.GotoLabel(label);
    }


	public void GetAnswer(DialogDto context, string message, string zoneName)
	{
        if (context.Messages == null)
            context.Messages = new List<MessageDto>();
        AddUsersAnswersToContext(context, message);
        context.UserMessage = message;

		LoadingIndicator.SetActive(true);
		// Upload dialog and save id
		var coroutine = RequestController.ServerStartParsing(context, (id) => StartServerPolling(id, message, zoneName));
		StartCoroutine(coroutine);
	}

    private void AddUsersAnswersToContext(DialogDto context, string message)
    {
        GameManager subprogram = RenSharp;
        int level = 0;

        while(subprogram != null)
        {
            level++;

            List<string> messages = subprogram.GetLastMessages(count: 2);

            foreach(string msg in messages)
            {
                var msgDto = new MessageDto()
                {
                    IsPlayerMessage = false,
                    Text = msg
                };
                context.Messages.Add(msgDto);
            }

            if(subprogram.SubProgram == null)
            {
                subprogram.LastUserMessage = message;
				break;
            }

			string playerMessage = subprogram.LastUserMessage;
			if (playerMessage != null && playerMessage != "")
                AddUserMessageToDto(context, playerMessage);	
			

			subprogram = subprogram.SubProgram;
        }
    }

    private void AddUserMessageToDto(DialogDto context, string text)
    {
		var userMsg = new MessageDto()
		{
			IsPlayerMessage = true,
			Text = text
		};
		context.Messages.Add(userMsg);
	}

    private void StartServerPolling(int id, string message, string zoneName)
    {
        if(id == -1)
        {
            HandleAnswer(null, zoneName);
            return;
        }
        Debug.Log($"Polling by id {id} started");
        var coroutine = RequestController.GetResponseFromServer(id, message, x => HandleAnswer(x, zoneName));
        StartCoroutine(coroutine);
    }

    private void HandleAnswer(string answer, string zoneName)
    {
		LoadingIndicator.SetActive(false);
		Debug.Log("Final answer: " + answer);

        /*bool cached = TryJumpToCachedLabel(answer);
        if (cached)
        {
            IsPaused = false;
			ProccessCommandAfterPause();
			return;
		}*/

		if (answer == "" || answer == null || answer == "$WAIT_MORE" || answer == "\"$WAIT_MORE\"")
        {
			Debug.Log("Мы в глубокой жопе");
            IsPaused = false;
            GotoLabel(zoneName + "_offline");
			ProccessCommandAfterPause();
			return;
		}

        bool continueDialog = true;
        if (answer.ToLower().EndsWith("$end_of_game"))
            continueDialog = false;

		Dictionary<string, CommandAttribute> characterNameAttr = ParseCharacterName(zoneName);

		if (answer.ToLower().EndsWith("$elizstun"))
        {
            LoadLabelFromServer("elizStun", answer, characterNameAttr);
			return;
        }
        if(answer.ToLower().EndsWith("$johnstun"))
        {
            LoadLabelFromServer("johnStun", answer, characterNameAttr);
            return;
        }
		if (answer.ToLower().EndsWith("$zeronstun"))
		{
			LoadLabelFromServer("zeronStun", answer, characterNameAttr);
			return;
		}
		if (answer.ToLower().EndsWith("$leaveresistance") || answer.ToLower().EndsWith("$leave"))
		{
			LoadLabelFromServer("leaveResistance", answer, characterNameAttr);
			return;
		}


		if (answer.ToLower().EndsWith("$real_end_of_game"))
        {
			answer = DeleteCommands(answer);
			RealEndOfGameSubprogram(answer);
            IsPaused = false;
			ProccessCommandAfterPause();
			return;
        }

        // TODO
		answer = DeleteCommands(answer);




		List<Command> commands = new List<Command>()
        {
            new Command() { Type = CommandType.Label, Value = "main" },
            new Command() { Type = CommandType.Message, Value = answer, Attributes = characterNameAttr },
		};

        if (continueDialog)
            commands.Add(new Command() { Type = CommandType.Textinput, Value = zoneName });
        commands.Add(new Command() { Type = CommandType.EndOfGame });

        SetProgramLines(commands);

		GameManager subprogram = new GameManager(commands, Writer);
        RenSharp.RunLowLevelSubprogram(subprogram);
        IsPaused = false;
		ProccessCommandAfterPause();
	}

    private Dictionary<string, CommandAttribute> ParseCharacterName(string location)
    {
		string characterName = "";
		if (location == "zeronDialog")
			characterName = "<color=#ff9900>Зерон";
		else if (location == "afterOperationEliz1" || location == "newDayElizDialog")
			characterName = "Элизабет";
		else if (location == "johnElevator")
			characterName = "<color=#ff9900>Джон";

		Dictionary<string, CommandAttribute> characterNameAttr = new Dictionary<string, CommandAttribute>()
		{
			{ "name", new DisplayNameAttribute(characterName) }
		};

        return characterNameAttr;
	}

    private void LoadLabelFromServer(string label, string message, Dictionary<string, CommandAttribute> characterName)
    {
		GotoLabel(label);
		message = DeleteCommands(message);
        if(message != null && message != "")
        {
			SubprogramWithMessage(message, characterName);
		}
		IsPaused = false;
		ProccessCommandAfterPause();
		return;
	}

    private void SubprogramWithMessage(string message, Dictionary<string, CommandAttribute> characterName)
    {
        message = DeleteCommands(message);
		List<Command> commands = new List<Command>()
		{
			new Command() { Type = CommandType.Label, Value = "main" },
			new Command() { Type = CommandType.Message, Value = message, Attributes = characterName },
			new Command() { Type = CommandType.EndOfGame },
		};
		SetProgramLines(commands);
		GameManager subprogram = new GameManager(commands, Writer);

		RenSharp.RunSubprogram(subprogram);
		IsPaused = false;
	}

    private void RealEndOfGameSubprogram(string message)
    {
        List<Command> commands = new List<Command>()
        {
            new Command() { Type = CommandType.Label, Value = "main" },
            new Command() { Type = CommandType.Message, Value = message },
            new Command() { Type = CommandType.RealEndOfGame },
		};
		SetProgramLines(commands);
		GameManager subprogram = new GameManager(commands, Writer);

        RenSharp.RunSubprogram(subprogram);
        IsPaused = false;
	}

	private string DeleteCommands(string answer)
	{
		answer = Regex.Replace(answer, "\\$END_OF_GAME", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$REAL_END_OF_GAME", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$label elizstun", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$elizstun", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$zeronstun", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$leaveresistance", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$johnstun", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$label", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "\\$leave", "", RegexOptions.IgnoreCase);
		answer = Regex.Replace(answer, "elizstun", "", RegexOptions.IgnoreCase);

		return answer;
	}


    private void SetProgramLines(List<Command> program)
    {
        for(int i = 0; i < program.Count; i++)
        {
            program[i].Line = i + 1;
        }
    }

	#region Helpers
	private void ChangeBG(string background)
    {
        Background.color = Color.white;
        if (background == "bar")
        {
            Background.sprite = BarSprite;
        }
        else if (background == "surgery") 
        {
            Background.sprite = Surgery;
        }
        else if(background == "reception")
        {
            Background.sprite = Reception;
        }
        else if(background == "elevator")
        {
            Background.sprite = Elevator;
        }
        else if(background == "precompany")
        {
            Background.sprite = PreCompany;
        }
        else if(background == "leaderoffice1")
        {
            Background.sprite = LeaderOffice1;
        }
		else if (background == "leaderoffice2")
		{
			Background.sprite = LeaderOffice2;
		}
		else if (background == "leaderoffice3")
		{
			Background.sprite = LeaderOffice3;
		}
		else if (background == "leaderoffice4")
		{
			Background.sprite = LeaderOffice4;
		}
		else if(background == "black")
        {
            Background.sprite = null;
            Background.color = Color.black;
        }
        else if(background == "white")
        {
            Background.sprite = null;
            Background.color = Color.white;
        }
    }


    private void ChangeCharacter(string characterName)
    {
        if(characterName == "alex")
        {
            Character.sprite = AlexSprite;
        }
        else if(characterName == "skiv")
        {
            Character.sprite = SkivFace;
        }
        else if(characterName == "mc_sitting")
        {
            Character.sprite = MC_Sitting;
        }
        else if(characterName == "john")
        {
            Character.sprite = JohnSprite;
        }
        else if(characterName == "sara")
        {
            Character.sprite = SaraSprite;
        }
        else if(characterName == "zeron")
        {
            Character.sprite = ZeronSprite;
        }
    }

	private void ChangeCharacter2(string characterName)
	{
		if (characterName == "alex")
		{
			Character2.sprite = AlexSprite;
		}
		else if (characterName == "skiv")
		{
			Character2.sprite = SkivFace;
		}
		else if (characterName == "mc_sitting")
		{
			Character2.sprite = MC_Sitting;
		}
		else if (characterName == "john")
		{
			Character2.sprite = JohnSprite;
		}
		else if (characterName == "sara")
		{
			Character2.sprite = SaraSprite;
		}
		else if (characterName == "zeron")
		{
			Character2.sprite = ZeronSprite;
		}
	}

	#endregion

}
