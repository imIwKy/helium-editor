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
    private readonly string filePath;
    private readonly List<string> fileContent = [];
    private readonly List<LineFlags> lineFlags = [];
    private readonly Regex allowedCharacters = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*()_+-=,.<>/?;:'""\[\]{}|\\]+$");
    //private const int TAB_LENGTH = 4;
    private const int FIRST_ARROW_KEY = 37;
    private const int LAST_ARROW_KEY = 40;

    private void Edit()
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(true);
            (int cursorX, int cursorY) = Console.GetCursorPosition();
            CheckTextWrapping(cursorX, cursorY);

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
            if(!allowedCharacters.IsMatch(input.ToString())) 
            {
                continue;
            }

            AddCharacter(input.ToString());


        }
        while(keyInfo.Key != ConsoleKey.Escape);

        Exit();
    }

    private void AddCharacter(string input)
    {
        (int cursorX, int cursorY) = Console.GetCursorPosition();

        if(cursorX == Console.WindowWidth - 1)
        {
            if(lineFlags[cursorY + 1] == LineFlags.Wrapped)
            {
                cursorY++;
                cursorX = 0;
                Console.SetCursorPosition(cursorX, cursorY);
                RedrawLines(cursorX, cursorY);
                return;
            }

            Console.Write(input);
            fileContent[cursorY] = fileContent[cursorY].Insert(cursorX, input);
            cursorY++;
            cursorX = 0;
            fileContent.Insert(cursorY, "");
            lineFlags.Insert(cursorY, LineFlags.Wrapped);
            RedrawLines(cursorX, cursorY);
            return;
        }

        if(fileContent[cursorY].Length == Console.WindowWidth)
        {
            fileContent[cursorY] = fileContent[cursorY].Insert(cursorX, input);
            string basePart = fileContent[cursorY].Substring(0, Console.WindowWidth);
            string overflownPart = fileContent[cursorY].Substring(Console.WindowWidth);

            if(cursorY == fileContent.Count - 1 || lineFlags[cursorY + 1] != LineFlags.Wrapped)
            {
                fileContent[cursorY] = basePart;
                fileContent.Insert(cursorY + 1, overflownPart);
                lineFlags.Insert(cursorY + 1, LineFlags.Wrapped);
                RedrawLines(cursorX, cursorY);
                return;
            }

            if(lineFlags[cursorY + 1] == LineFlags.Wrapped)
            {
                fileContent[cursorY] = basePart;
                fileContent[cursorY + 1] = fileContent[cursorY + 1].Insert(0, overflownPart);
                cursorX++;
                Console.SetCursorPosition(cursorX, cursorY);
                RedrawLines(cursorX, cursorY);
                return;
            }

            throw new Exception("Undefined behaviour");
        }

        Console.Write(input);
        fileContent[cursorY] = fileContent[cursorY].Insert(cursorX, input);
        cursorX++;
        Console.SetCursorPosition(cursorX, cursorY);
        Console.Write(fileContent[cursorY].Substring(cursorX));
        Console.SetCursorPosition(cursorX, cursorY);
    }

    private void Exit()
    {
        Console.Clear();

        while(true)
        {
            Console.WriteLine("Do you want to save the changes?(y/n)");
            ConsoleKey answer = Console.ReadKey().Key;

            if(answer == ConsoleKey.Y)
            {
                FileManager.Save(fileContent, filePath);
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
                    {
                        cursorX = Console.WindowWidth - 1;
                    }
                    else
                    {
                        cursorX = fileContent[cursorY].Length;
                    }
                    
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
            case ConsoleKey.DownArrow:
                if(cursorY == fileContent.Count - 1) {break;}
                else
                {
                    cursorY += 1; 
                    if(fileContent[cursorY].Length == Console.WindowWidth)
                    {
                        cursorX = fileContent[cursorY].Length - 1;
                    }
                    else
                    {
                        cursorX = fileContent[cursorY].Length;
                    }

                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.UpArrow:
                if(cursorY == 0) {break;}
                else
                {
                    cursorY -= 1;
                    if(fileContent[cursorY].Length == Console.WindowWidth)
                    {
                        cursorX = fileContent[cursorY].Length - 1;
                    }
                    else
                    {
                        cursorX = fileContent[cursorY].Length;
                    }
                    
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
            if(fileContent[i].Length == Console.WindowWidth)
            {
                Console.Write(fileContent[i]);
                lineFlags.Add(LineFlags.Normal);
                continue;
            }
            else if(fileContent[i].Length > Console.WindowWidth)
            {
                WrapText(i);
                Console.WriteLine(fileContent[i].Substring(0, Console.WindowWidth));
                Console.SetCursorPosition(Console.GetCursorPosition().Left, Console.GetCursorPosition().Top - 1);
                continue;
            }

            Console.WriteLine(fileContent[i]);
            lineFlags.Add(LineFlags.Normal);
        }

        Console.SetCursorPosition(0,0);
    }

    private void CheckTextWrapping(int x, int y)
    {
        for (int i = 0; i < fileContent.Count; i++)
        {
            if(fileContent[i].Length > Console.WindowWidth)
            {
                WrapText(i);
                RedrawLines(x, y);
            }
            else if(lineFlags[i] == LineFlags.Wrapped && fileContent[i - 1].Length < Console.WindowWidth)
            {
                UnwrapText(i);
                RedrawLines(x, y);
            }
        }
    }

    private void WrapText(int lineIndex)
    {
        string basePart = fileContent[lineIndex].Substring(0, Console.WindowWidth);
        string overflownPart = fileContent[lineIndex].Substring(Console.WindowWidth);
        fileContent[lineIndex] = basePart;
        lineIndex++;
        fileContent.Insert(lineIndex, overflownPart);
        lineFlags.Insert(lineIndex, LineFlags.Wrapped);
    }

    private void UnwrapText(int lineIndex)
    {
        if(fileContent[lineIndex - 1].Length + fileContent[lineIndex].Length < Console.WindowWidth)
        {
            fileContent[lineIndex - 1] += fileContent[lineIndex];
            fileContent.RemoveAt(lineIndex);
            lineFlags.RemoveAt(lineIndex);
        }
        else if(fileContent[lineIndex - 1].Length + fileContent[lineIndex].Length == Console.WindowWidth)
        {
            fileContent[lineIndex - 1] += fileContent[lineIndex];
            fileContent.RemoveAt(lineIndex);
            lineFlags.RemoveAt(lineIndex);
        }
        else
        {
            string partToWrapBack = fileContent[lineIndex].Substring(0, Console.WindowWidth - fileContent[lineIndex - 1].Length);
            fileContent[lineIndex - 1] += partToWrapBack;
            fileContent[lineIndex] = fileContent[lineIndex].Remove(0, partToWrapBack.Length); 
        }
    }

    private void RedrawLines(int x, int y)
    {
        Console.ForegroundColor = ConsoleColor.White;
        int oldX = x;
        int oldY = y;

        for(int i = 0; i < fileContent.Count; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, i);
            Console.WriteLine(fileContent[i]);
        }

        Console.Write(new string(' ', fileContent.Last().Length));
        Console.SetCursorPosition(0,0);
        Console.SetCursorPosition(oldX, oldY);
    }

    public Editor(List<string> content, string path)
    {
        filePath = path;
        fileContent = content;
        DisplayContent();
        Edit();
    }
}
