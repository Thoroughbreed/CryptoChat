using System;
using System.Collections.Generic;

namespace Console_Client.Utils
{
    public class ASCII
    {
        public List<string> MenuItems { get; }
        public event Action<int> IndexChanged;

        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly int _offset;
        private readonly bool _clear;
        
        /// <summary>
        /// Jolly Roger ASCII
        /// </summary>
        public static void JollyRoger()
        {
            Console.Clear();
            Console.WriteLine(@"8888888888888888888888888888888888888888888888888888888888888");
            Console.WriteLine(@"8888888888888888888888888888888888888888888888888888888888888");
            Console.WriteLine(@"8888888888888888888888888P""""  """"98888888888888888888888888888");
            Console.WriteLine(@"8888888888888888P""88888P          988888""98888888888888888888");
            Console.WriteLine(@"8888888888888888  ""9888            888P""  8888888888888888888");
            Console.WriteLine(@"888888888888888888bo ""9  d8o  o8b  P"" od888888888888888888888");
            Console.WriteLine(@"888888888888888888888bob 98""  ""8P dod888888888888888888888888");
            Console.WriteLine(@"888888888888888888888888    db    888888888888888888888888888");
            Console.WriteLine(@"88888888888888888888888888      88888888888888888888888888888");
            Console.WriteLine(@"88888888888888888888888P""9bo  odP""988888888888888888888888888");
            Console.WriteLine(@"88888888888888888888P"" od88888888bo ""988888888888888888888888");
            Console.WriteLine(@"888888888888888888   d88888888888888b   888888888888888888888");
            Console.WriteLine(@"8888888888888888888oo8888888888888888oo8888888888888888888888");
            Console.WriteLine(@"8888888888888888888888888888888888888888888888888888888888888");
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               WELCOME TO THE ENCRYPTED CHAT               ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        }
        
        /// <summary>
        /// Initiates the "fancy menu"
        /// </summary>
        /// <param name="menuItems">Menu items (TList)</param>
        /// <param name="screenWidth">Optional - console width</param>
        /// <param name="screenHeight">Optional - console height</param>
        /// <param name="offset">Optional - x offset</param>
        /// <param name="clear">Optional - clears console (default true)</param>
        public ASCII(List<string> menuItems, int screenWidth = -1, int screenHeight = -1, int offset = 0, bool clear = true)
        {
            MenuItems = menuItems;
            _screenWidth = screenWidth == -1 ? Console.WindowWidth : screenWidth;
            _screenHeight = screenHeight == -1 ? Console.WindowHeight : screenHeight;
            _offset = offset;
            _clear = clear;
        }

        /// <summary>
        /// Draws the menu itself
        /// </summary>
        /// <returns>Integer for the list index</returns>
        public int Draw()
        {
            if (_clear)
                Console.Clear();

            Console.CursorVisible = false;
            int posY = (_screenHeight - MenuItems.Count) / 2;
            int selectedItemIndex = 0;

            #region Input validation
            if (posY <= 0)
                throw new InvalidOperationException("There is not enough space");
            if (MenuItems.TrueForAll(i => i.Length > _screenWidth - 4))
                throw new InvalidOperationException("One of the items are longer than the screen width");
            #endregion

            while (true)
            {
                // Centers text vertically
                posY = (_screenHeight - MenuItems.Count) / 2;
                foreach (var item in MenuItems)
                {
                    // Centers text horizontally
                    int posX = (_screenWidth - item.Length) / 2 + _offset;
                    if (MenuItems.IndexOf(item) == selectedItemIndex)
                    {
                        Console.SetCursorPosition(posX, posY++);
                        Console.Write($"[ ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{item}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($" ]");
                        IndexChanged?.Invoke(selectedItemIndex);
                    }
                    else
                    {
                        Console.SetCursorPosition(posX, posY++);
                        Console.WriteLine($"  {item}  ");
                    }
                }

                #region Controls
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedItemIndex <= 0)
                            selectedItemIndex = MenuItems.Count - 1;
                        else
                            selectedItemIndex--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (selectedItemIndex >= MenuItems.Count - 1)
                            selectedItemIndex = 0;
                        else
                            selectedItemIndex++;
                        break;
                    case ConsoleKey.Escape:
                        return 9;
                    case ConsoleKey.X:
                        return 9;
                #endregion
                    case ConsoleKey.Enter:
                        return selectedItemIndex;
                    case ConsoleKey.Spacebar:
                        return selectedItemIndex;
                }
            }
        }

        /// <summary>
        /// DOOM ASCII
        /// </summary>
        public static void DOOM()
        {
            int posX = (Console.WindowWidth - 71) / 2;
            int posY = 5;
            Console.Clear();

            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("=================     ===============     ===============   ========  ========");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("\\\\ . . . . . . .\\\\   //. . . . . . .\\\\   //. . . . . . .\\\\  \\\\. . .\\\\// . . //");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||. . ._____. . .|| ||. . ._____. . .|| ||. . ._____. . .|| || . . .\\/ . . .||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("|| . .||   ||. . || || . .||   ||. . || || . .||   ||. . || ||. . . . . . . ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||. . ||   || . .|| ||. . ||   || . .|| ||. . ||   || . .|| || . | . . . . .||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("|| . .||   ||. _-|| ||-_ .||   ||. . || || . .||   ||. _-|| ||-_.|\\ . . . . ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||. . ||   ||-'  || ||  `-||   || . .|| ||. . ||   ||-'  || ||  `|\\_ . .|. .||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("|| . _||   ||    || ||    ||   ||_ . || || . _||   ||    || ||   |\\ `-_/| . ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||_-' ||  .|/    || ||    \\|.  || `-_|| ||_-' ||  .|/    || ||   | \\  / |-_.||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||    ||_-'      || ||      `-_||    || ||    ||_-'      || ||   | \\  / |  `||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||    `'         || ||         `'    || ||    `'         || ||   | \\  / |   ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||            .===' `===.         .==='.`===.         .===' /==. |  \\/  |   ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||         .=='   \\_|-_ `===. .==='   _|_   `===. .===' _-|/   `==  \\/  |   ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||      .=='    _-'    `-_  `='    _-'   `-_    `='  _-'   `-_  /|  \\/  |   ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||   .=='    _-'          `-__\\._-'         `-_./__-'         `' |. /|  |   ||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("||.=='    _-'                                                     `' |  /==.||");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("== '    _-'                                                           \\/   `==");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine("\\   _ -'                                                               `-_   /");
            Console.SetCursorPosition(posX, posY++);
            Console.WriteLine(" `''                                                                     ``'");
        }
    }
}