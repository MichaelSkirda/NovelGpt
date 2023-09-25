using RenSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenSharp
{
    public class FileReader
    {

        public FileReader()
        { 
        
        }

        public List<Command> CompileFile(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            return CompileFile(lines);
        }

        public List<Command> CompileFile(string[] lines)
        {
			List<Command> commands = new List<Command>();

			int lineNum = 1;

			for (int i = 0; i < lines.Length; i++)
			{
				try
				{
					lineNum = i + 1;
					string line = lines[i];
					line = line.DeleteAfter("//"); // Delete comments
					line = line.Trim();

					Command command = ParseCommand(line);
					command.Line = lineNum;
					commands.Add(command);
				}
				catch (Exception ex)
				{
					Exception exception = new Exception($"Exception was thrown at line {lineNum}", ex);
					throw exception;
				}
			}

			return commands;
		}


		private Command ParseCommand(string line)
        {
            var command = new Command();

            string[] lineWords = line.Split(' ');
            string[] lineArgs = lineWords.Skip(1).ToArray();
            string? firstArgument;
            string keyword = lineWords[0];
            if (lineWords.Length >= 2)
                firstArgument = lineWords[1];
            else
                firstArgument = null;


            if (line.StartsWith("\""))
            {
                command.Type = CommandType.Message;
                command.Value = line.GetStringBetween("\"", "\"");
                command.Attributes = AttributeParser.ParseMessageAttributes(line);
            }
            else if (keyword == "$label")
            {
                command.Type = CommandType.Label;
                if (lineArgs.Length >= 2)
                    throw new Exception($"$label must have exactly 1 argument. Unexpected argument {lineArgs[1]}");

                command.Value = firstArgument;
            }
            else if (keyword == "$choice")
            {
                command.Type = CommandType.Choice;
                command.Value = firstArgument;
            }
            else if(keyword == "$goto")
            {
                command.Type = CommandType.Goto;

                if (firstArgument == null)
                    throw new ArgumentException("$goto must have atleast 1 argument (label to jump)");
                command.Value = firstArgument;
            }
            else if(keyword == "$changeBG")
            {
                command.Type = CommandType.ChangeBG;
                command.Value = firstArgument;
            }
            else if(keyword == "$changeChar")
            {
                command.Type = CommandType.ChangeCharacter;
                command.Value = firstArgument;
                command.Attributes = AttributeParser.ParseValueAttributes(line);
            }
            else if(keyword == "$hideChar")
            {
                command.Type = CommandType.HideCharacter;
            }
			else if (keyword == "$changeChar2")
			{
				command.Type = CommandType.ChangeCharacter2;
				command.Value = firstArgument;
				command.Attributes = AttributeParser.ParseValueAttributes(line);
			}
			else if (keyword == "$hideChar2")
			{
				command.Type = CommandType.HideCharacter2;
			}
			else if(keyword == "$END_OF_GAME")
            {
                command.Type = CommandType.EndOfGame;
                command.Value = firstArgument;
            }
            else if(keyword == "$textinput")
            {
                command.Type = CommandType.Textinput;
                command.Value = firstArgument;
            }
            else if(keyword == "$internetCheck")
            {
                command.Type = CommandType.InternetCheck;
                command.Value = firstArgument;
            }
            else if(keyword == "$wait")
            {
                command.Type = CommandType.Wait;

                if (firstArgument == null)
                    throw new ArgumentException("$wait must have atleast 1 argument (milliseconds to wait)");
                command.Value = firstArgument;
            }
            else if(keyword == "$character")
            {
                ParseCharacter(command, line);
            }
            else if(keyword == "$config")
            {
                command.Type = CommandType.Config;
                command.Attributes = AttributeParser.ParseConfigAttributes(line);
            }
            else if(keyword == "$playSound")
            {
                command.Type = CommandType.PlaySound;
                command.Value = firstArgument;
            }
            else if(line == "")
            {
                command.Type = CommandType.Nop;
            }

            return command;
        }

        private void ParseCharacter(Command command, string line)
        {
            command.Type = CommandType.Character;
            command.Value = line.GetStringBetween("$character ", " ");

            // english leteers and nums 0-0
            if (Regex.IsMatch(command.Value, @"^[А-яЁёa-zA-Z0-9]+$") == false)
                throw new Exception("Character may contains only letters and numbers");

            string[] attributes = line.Split(' ')
                .Skip(2) // First - input, Second - character name
                .ToArray();

            command.Attributes = AttributeParser.ParseAttributes(attributes);
        }



    }
}