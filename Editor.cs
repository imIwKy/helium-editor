using System.Text.RegularExpressions;

namespace helium_editor;

enum LineFlags
{
    Normal,
    Modified,
    Wrapped
}

class Editor
{
    private string filePath;
    private List<string> fileContent = new List<string>();
    private List<LineFlags> lineFlags = new List<LineFlags>();
    private Regex allowedCharacters = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*()_+-=,.<>/?;:'""\[\]{}|\\]+$");
    private const int TAB_LENGTH = 4;
    private const int FIRST_ARROW_KEY = 37;
    private const int LAST_ARROW_KEY = 40;

    private void Edit()
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);

            if((int)keyInfo.Key >= FIRST_ARROW_KEY && (int)keyInfo.Key <= LAST_ARROW_KEY)
            {
                MoveCursor(keyInfo.Key);
                continue;
            }

            if(char.IsControl(keyInfo.KeyChar)) 
            {
                //HandleSpecialCharacters(keyInfo.Key);
                continue;
            }

            char input = keyInfo.KeyChar;
            if(!allowedCharacters.IsMatch(input.ToString())) continue;

            //AddCharacter(input);


        }
        while(keyInfo.Key != ConsoleKey.Escape);

        Exit();
    }

    private void Exit()
    {
        FileManager fileManager = new FileManager();
        Console.Clear();

        while(true)
        {
            Console.WriteLine("Do you want to save the changes?(y/n)");
            ConsoleKey answer = Console.ReadKey().Key;

            if(answer == ConsoleKey.Y)
            {
                fileManager.Save(fileContent, filePath);
                Console.Clear();
                break;
            }
            else if(answer == ConsoleKey.N)
            {
                Console.Clear();
                break;
            }

            Console.Clear();
        }

    }

    private void MoveCursor(ConsoleKey key)
    {
        (int cursorX, int cursorY) = Console.GetCursorPosition();
        
        switch(key)
        {
            case ConsoleKey.LeftArrow:
                if(cursorX == 0 && cursorY > 0)
                {
                    cursorY--;

                    if(fileContent[cursorY].Length >= Console.WindowWidth)
                    cursorX = Console.WindowWidth - 1;
                    else
                    cursorX = fileContent[cursorY].Length;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                else if(cursorX > 0)
                {
                    cursorX--;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.RightArrow:
                if(cursorX == fileContent[cursorY].Length && cursorY < fileContent.Count - 1 || cursorX == Console.WindowWidth - 1)
                {
                    cursorX = 0;
                    cursorY++;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                else if(cursorX < fileContent[cursorY].Length)
                {
                    cursorX++;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
        }
    }

    private void DisplayContent()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;

        for(int i = 0; i < fileContent.Count; i++)
        {
            if(fileContent[i].Length > Console.WindowWidth)
            {
                string basePart = fileContent[i].Substring(0, Console.WindowWidth);
                string overflownPart = fileContent[i].Substring(Console.WindowWidth);
                fileContent[i] = basePart;
                fileContent.Insert(i + 1, overflownPart);
                lineFlags.Add(LineFlags.Normal);
                lineFlags.Add(LineFlags.Wrapped);
                Console.WriteLine(basePart);
                Console.SetCursorPosition(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top - 1);
                continue;
            }

            Console.WriteLine(fileContent[i]);
            lineFlags.Add(LineFlags.Normal);
        }

        Console.SetCursorPosition(0,0);
    }

    public Editor(List<string> content, string path)
    {
        filePath = path;
        fileContent = content;
        DisplayContent();
        Edit();
    }
}
