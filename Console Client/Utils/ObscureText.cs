using System;
using System.Text;

namespace Console_Client.Utils
{
    public class ObscureText
    {
        /// <summary>
        /// Simple method to hide user-input in the console and replace them with a generic char.
        /// </summary>
        /// <returns>The string the user inputs</returns>
        internal string HideInput()
        {
            StringBuilder input = new();
            while (true)
            {
                // Take note of current curser position
                int x = Console.CursorLeft;
                int y = Console.CursorTop;
                ConsoleKeyInfo key = Console.ReadKey(true); // Grabs the keyinput without printing it
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                // Deletes  if user presses backspace
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.SetCursorPosition(x - 1, y);
                    Console.Write(" ");
                    Console.SetCursorPosition(x - 1, y);
                }
                // If somehow you get a key less than 32 ASCII or above 126 ASCII
                // It intercepts it and logs it in log-file
                else if( key.KeyChar < 32 || key.KeyChar > 126 )
                {
                    // FileLogger not imported in this project.
                    //FileLogger.WriteTo($"Weird input detected - {key.KeyChar}", null);
                }
                // All other key-inputs are put into the string
                // And input is replaced with 
                else if (key.Key != ConsoleKey.Backspace)
                {
                    input.Append(key.KeyChar);
                    Console.Write("");
                }
            }
            return input.ToString();
        }
    }
}