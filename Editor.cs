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
    private List<string> fileContent = new List<string>();
    private List<LineFlags> lineFlags = new List<LineFlags>();
    private Regex allowedCharacters = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*()_+-=,.<>/?;:'""\[\]{}|\\]+$");
    private const int TAB_LENGTH = 4;
    private const int FIRST_ARROW_KEY = 37;
    private const int LAST_ARROW_KEY = 40;

    private void Edit(string tempFilePath)
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

    private void CheckTextWrapping()
    {
        FileManager fileManager = new FileManager();

        for(int i = 0; i < fileContent.Count; i++)
        {
            if(fileContent[i].Length > Console.WindowWidth)
            {
                string overflownPart = fileContent[i].Substring(Console.WindowWidth);
                fileContent.Insert(i + 1, overflownPart);
                fileContent[i] = fileContent[i].Remove(Console.WindowWidth);
                lineFlags.Insert(i + 1, LineFlags.Wrapped);
                continue;
            }

            if(fileContent[i].Length < Console.WindowWidth && lineFlags[i] == LineFlags.Wrapped)
            {
                int combinedLength = fileContent[i].Length + fileContent[i - 1].Length;

                if(combinedLength <= Console.WindowWidth)
                {
                    fileContent[i - 1] = fileContent[i - 1] + fileContent[i];
                    fileContent.RemoveAt(i);
                    lineFlags.RemoveAt(i);
                    continue;
                }
                else if(combinedLength > Console.WindowWidth && fileContent[i - 1].Length < Console.WindowWidth)
                {
                    string avaliblePart = fileContent[i].Substring(0, Console.WindowWidth - fileContent[i - 1].Length);
                    fileContent[i] = fileContent[i].Remove(0, Console.WindowWidth - fileContent[i - 1].Length);
                    fileContent[i - 1] = fileContent[i - 1] + avaliblePart;
                }

            }
        }
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
                fileManager.Save(fileContent, Program.filePath);
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
                    CheckTextWrapping();
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
                    CheckTextWrapping();
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

    public Editor()
    {
        FileManager fileManager = new FileManager();
        fileContent = fileManager.Load();
        
        for(int i = 0; i < fileContent.Count; i++)
        {
            lineFlags.Add(LineFlags.Normal);
        }
        Edit(fileManager.tempFilePath);
    }
}
