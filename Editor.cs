using System.Text.RegularExpressions;

namespace helium_editor;

class Editor
{
    private List<string> page = new List<string>();
    private string filePath;
    private Regex allowedCharacters = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*()_+-=,.<>/?;:'""\[\]{}|\\]+$");

    public void Edit(string tempFilePath)
    {
        ConsoleKeyInfo keyInfo;

        do
        {
            keyInfo = Console.ReadKey(true);

            if((int)keyInfo.Key >= 37 && (int)keyInfo.Key <= 40)
            {
                MoveCursor(keyInfo.Key);
                continue;
            }

            if(char.IsControl(keyInfo.KeyChar)) continue;

            char input = keyInfo.KeyChar;
            if(!allowedCharacters.IsMatch(input.ToString())) continue;

            AddCharacter(input);


        }
        while(keyInfo.Key != ConsoleKey.Escape);

        Exit();
    }

    public void DisplayContent(string tempFilePath)
    {
        StreamReader file = new StreamReader(tempFilePath);

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;

        string? line = file.ReadLine();

        while(line != null)
        {
            page.Add(line);
            Console.WriteLine(line);
            line = file.ReadLine();
        }

        AddFooter();
        file.Close();
    }

    private void AddFooter()
    {
        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(0,0);
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
                fileManager.Save(page, filePath);
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
                    cursorX = page[cursorY - 1].Length;
                    cursorY -= 1;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                else if(cursorX > 0)
                {
                    cursorX -= 1;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.RightArrow:
                if(cursorX == page[cursorY].Length && cursorY < page.Count - 1)
                {
                    cursorX = 0;
                    cursorY += 1;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                else if(cursorX < page[cursorY].Length)
                {
                    cursorX += 1;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.DownArrow:
                if(cursorY == page.Count - 1) break;
                else
                {
                    cursorY += 1; 
                    cursorX = page[cursorY].Length; 
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.UpArrow:
                if(cursorY == 0) break;
                else
                {
                    cursorY -= 1;
                    cursorX = page[cursorY].Length;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
        }
    }

    private void AddCharacter(char input)
    {
        (int x, int y) = Console.GetCursorPosition();

        if(x == page[y].Length && x < Console.WindowWidth)
        {
            page[y] += input;
            Console.Write(input);
        }
        else if(x == Console.WindowWidth)
        {
            page.Insert(y, input.ToString());
            x = 0;
            y += 1;
            Console.SetCursorPosition(x, y);
        }
    }

    public Editor(string path)
    {
        filePath = path;
    }
}