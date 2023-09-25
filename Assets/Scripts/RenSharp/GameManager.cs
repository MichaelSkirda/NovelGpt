using RenSharp.Interfaces;
using RenSharp.Models;
using RenSharp.Models.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp
{
    public class GameManager
    {
        private List<Command> Program { get; set; }
        public GameManager SubProgram { get; private set; }
        private IWriter Writer { get; set; }

        private List<Command> Labels { get; set; }
        private int CurrentLine { get; set; }
        private Dictionary<string, CommandAttribute> DefaultAttributes { get; set; }
            = new();
        private List<Character> Characters { get; set; } = new List<Character>();

        private List<CommandType> LegalTypes = new List<CommandType>()
        {
            CommandType.Goto,
            CommandType.ChangeBG,
            CommandType.EndOfGame,
            CommandType.RealEndOfGame,
            CommandType.Config,
            CommandType.Label,
            CommandType.ChangeCharacter,
            CommandType.HideCharacter,
            CommandType.Textinput,
            CommandType.Choice,
            CommandType.Nop,
            CommandType.InternetCheck,
            CommandType.ChangeCharacter2,
            CommandType.HideCharacter2,
            CommandType.PlaySound
        };

        public string LastUserMessage { get; set; }

        public GameManager(string filename, IWriter writer)
        {
            var reader = new FileReader();
            List<Command> program = reader.CompileFile(filename);

            SetupProgram(program, writer);
        }

        public GameManager(List<Command> program, IWriter writer)
        {
            SetupProgram(program, writer);
        }

        public GameManager(string[] lines, IWriter writer)
        {
            var reader = new FileReader();
            List<Command> program = reader.CompileFile(lines);

            SetupProgram(program, writer);

		}

		private void SetupProgram(List<Command> program, IWriter writer)
		{
			Writer = writer;
			Program = program;

			Labels = Program
				.Where(x => x.Type == CommandType.Label)
				.ToList();

			Characters = Program
				.Where(x => x.Type == CommandType.Character)
				.Select(x => new Character(name: x.Value, attributes: x.Attributes))
				.ToList();

			if (Program[0].Type == CommandType.Config)
				RunConfig(Program[0]);
			if (DefaultAttributes.GetAttributeOrDefault<DelayAttribute>() == null)
				DefaultAttributes.Add("delay", new DelayAttribute(delay: 0));

			GotoLabel("main");
		}

        public void RunSubprogram(GameManager subprogram)
        {
            SubProgram = subprogram;
        }

		internal void RunLowLevelSubprogram(GameManager program)
		{
            GameManager subprogram = this;
            while(true)
            {
                if (subprogram.SubProgram == null)
                {
					subprogram.SubProgram = program;
                    break;
				}
                subprogram = subprogram.SubProgram;
			}
		}

		public void RunConfig(Command config)
        {
            if (config.Type != CommandType.Config)
                throw new Exception($"Wrong cast at line {config.Line}! Expected $config");

            DefaultAttributes = config.Attributes;
        }

        public Command ReadNextLine()
        {
            Command command;

            if (SubProgram != null)
            {
                try
                {
					command = SubProgram.ReadNextLine();
				}
                catch
                {
                    command = new Command() { Type = CommandType.EndOfSubprogram };
                    SubProgram = null;
                    return command;
                }

				if (command.Type == CommandType.EndOfGame)
                {
                    SubProgram = null;
                    command.Type = CommandType.EndOfSubprogram;
                }
                if(command.Type == CommandType.RealEndOfGame)
                {
                    SubProgram = null;
                }
                return command;
            }

            CurrentLine++;
            if (CurrentLine > Program.Count)
                throw new Exception("End of game");

            command = Program[CurrentLine - 1];

            ExecuteCommand(command);

            return command;
        }



        public void ExecuteCommand(Command command)
        {
            if (command.Type == CommandType.Goto)
            {
                GotoLabel(command.Value);
            }
            else if (command.Type == CommandType.Message)
            {
                string message = command.Value;
                // Если установлен default character
                AddCharacterFromDefault(command);
                AddCharacterAttributes(command);
                AddDefaultAttributes(command);

                if(Writer != null)
                    Writer.Write(message, command.Attributes);
            }
            else if(LegalTypes.Contains(command.Type) == false)
            {
				throw new Exception("Unexpected command");
			}

        }

        private void AddCharacterFromDefault(Command command)
        {
            var characterAttr = DefaultAttributes.GetAttributeOrDefault<CharacterAttribute>();
            if (characterAttr == null)
                return;

            AddAttributeIfNotExist(command, characterAttr);
        }

        private void AddDefaultAttributes(Command command)
        {
            List<CommandAttribute> attributes = DefaultAttributes.Values.ToList();
            if (attributes == null)
                return;

            AddAttributesIfNotExists(command, attributes);
        }

        private void AddCharacterAttributes(Command command)
        {
            CharacterAttribute? characterAttr = command.Attributes.GetAttributeOrDefault<CharacterAttribute>();
            if (characterAttr == null)
                return;

            Character? character = Characters.FirstOrDefault(x => x.Name == characterAttr.Name);
            if (character == null)
                return;

            List<CommandAttribute> attributes = character.Attributes.Values.ToList();
            if (attributes == null)
                return;

            AddAttributesIfNotExists(command, attributes);
        }

        private void AddAttributesIfNotExists(Command command, List<CommandAttribute> attributes)
        {
            foreach(var attribute in attributes)
            {
                AddAttributeIfNotExist(command, attribute);
            }
        }

        private void AddAttributeIfNotExist(Command command, CommandAttribute attribute)
        {
            Type type = attribute.GetType();

            if(command.Attributes.GetAttributeOrDefault(type) == null)
            {
                string key = AttributeParser.ParseKey(type);
                command.Attributes.Add(key, attribute);
            }
        }

        public void GotoLabel(string name)
        {
            // Case sensitive
            Command label = Labels.FirstOrDefault(x => x.Value == name);
            if (label == null)
                throw new ArgumentException($"Label '{name}' wasn't found");

            CurrentLine = label.Line;
        }

        public void GotoLine(int line)
        {
            if (line > Program.Count)
                throw new Exception($"Line {line} not exists. This file is has {Program.Count} lines.");
            if (line <= 0)
                throw new Exception($"Line must be positive. Lins is: {line}");
            CurrentLine = line;
        }

        public List<string> GetLastMessages(int count)
        {
            List<string> result = new List<string>();
            int line = CurrentLine;

            // Идем в сторону 0 строчки собирая [count] сообщений
            for(int i = line - 1; i >= 0; i--)
            {
                try
                {
                    Command command = Program[i];
                    if (command == null || command.Type != CommandType.Message)
                        continue;

                    result.Add(command.Value);
                    if(result.Count >= count)
                        break;
                }
                catch
                {
                    break;
                }
            }
            result.Reverse();
            return result;
        }
		
	}
}
