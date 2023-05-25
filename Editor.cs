using System.Text.RegularExpressions;

namespace helium_editor;

class Editor
{
    private List<string> page = new List<string>();
    private string filePath;
    private Regex allowedCharacters = new Regex(@"^[a-zA-Z0-9\s!@#$%^&*()_+-=,.<>/?;:'""\[\]{}|\\]+$");
    private int chunkSize;
    private const int TAB_LENGTH = 4;
    private const int FIRST_ARROW_KEY = 37;
    private const int LAST_ARROW_KEY = 40;

    public void Edit(string tempFilePath)
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
                HandleSpecialCharacters(keyInfo.Key);
                continue;
            }

            char input = keyInfo.KeyChar;
            if(!allowedCharacters.IsMatch(input.ToString())) continue;

            AddCharacter(input);


        }
        while(keyInfo.Key != ConsoleKey.Escape);

        Exit();
    }

    public void DisplayContent(string tempFilePath)
    {
        chunkSize = Console.WindowHeight;
        StreamReader file = new StreamReader(tempFilePath);

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;

        string? line = file.ReadLine();
        int i = 0;

        while(line != null)
        {
            page.Add(line);

            if(i <= chunkSize) 
            {
                Console.WriteLine(line);
                i++;
            }
            
            line = file.ReadLine();
        }

        Console.SetCursorPosition(0,0);
        file.Close();
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
                    if(page[cursorY - 1].Length == Console.WindowWidth)
                    cursorX = Console.WindowWidth - 1;
                    else
                    cursorX = page[cursorY - 1].Length;

                    cursorY--;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                else if(cursorX > 0)
                {
                    cursorX--;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.RightArrow:
                if(cursorX == page[cursorY].Length && cursorY < page.Count - 1 || cursorX == Console.WindowWidth - 1)
                {
                    cursorX = 0;
                    cursorY++;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                else if(cursorX < page[cursorY].Length)
                {
                    cursorX++;
                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.DownArrow:
                if(cursorY == page.Count - 1) break;
                else if(cursorY >= Console.WindowHeight - 1)
                {
                    cursorY++;
                    cursorX = 0;
                    Console.SetCursorPosition(cursorX, cursorY);
                    Console.Write(page[cursorY]);
                    Console.SetCursorPosition(page[cursorY].Length, cursorY);
                }
                else
                {
                    cursorY++; 

                    if(page[cursorY].Length == Console.WindowWidth)
                    {cursorX = Console.WindowWidth - 1;}
                    else
                    {cursorX = page[cursorY].Length;}

                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
            case ConsoleKey.UpArrow:
                if(cursorY == 0) break;
                else
                {
                    cursorY--;

                    if(page[cursorY].Length == Console.WindowWidth)
                    {cursorX = Console.WindowWidth - 1;}
                    else
                    {cursorX = page[cursorY].Length;}

                    Console.SetCursorPosition(cursorX, cursorY);
                }
                break;
        }
    }

    private void AddCharacter(char input)
    {
        (int cursorX, int cursorY) = Console.GetCursorPosition();

        //Add to the end of the string.
        if(cursorX == page[cursorY].Length && cursorX < Console.WindowWidth - 1)
        {
            page[cursorY] += input;
            Console.Write(input);
            return;
        }
        //Adding to the end overflows
        else if(cursorX >= page[cursorY].Length && cursorX == Console.WindowWidth - 1)
        {
            cursorY++;
            int oldY = cursorY;
            Console.Write(input);
            page.Insert(cursorY, "");
            RedrawLines(cursorY);
            Console.SetCursorPosition(0, oldY);
            return;
        }
        //Add anywhere
        if(page[cursorY].Length < Console.WindowWidth)
        {
            page[cursorY] = page[cursorY].Insert(cursorX, input.ToString());
            Console.Write(input + page[cursorY].Substring(cursorX + 1));
            cursorX++;
            Console.SetCursorPosition(cursorX, cursorY);
            return;
        }
        //Adding anywhere overflows
        else if(page[cursorY].Length == Console.WindowWidth)
        {
            page.Insert(cursorY + 1, page[cursorY].Substring(page[cursorY].Length - 1));
            page[cursorY] = page[cursorY].Remove(page[cursorY].Length - 1);
            page[cursorY] = page[cursorY].Insert(cursorX, input.ToString());
            Console.Write(input + page[cursorY].Substring(cursorX + 1));
            cursorY++;
            int oldY = cursorY;
            RedrawLines(cursorY);
            Console.SetCursorPosition(0, oldY);
        }

    }

    private void RedrawLines(int cursorY)
    {
        chunkSize = Console.WindowHeight;

        for(int i = cursorY + 1; i <= chunkSize; i++)
        {
            Console.SetCursorPosition(0, i - 1);
            
            if(i >= page.Count)
            {
                Console.Write(' ');
                continue;
            }
            else
            {Console.Write(new string(' ', page[i].Length));}

            Console.SetCursorPosition(0, i - 1);
            Console.Write(page[i - 1]);
        }
    }

    private void HandleSpecialCharacters(ConsoleKey key)
    {
        (int cursorX, int cursorY) = Console.GetCursorPosition();

        switch(key)
        {
            case ConsoleKey.Enter:
                Console.Write(new string(' ', page[cursorY].Substring(cursorX).Length));
                page.Insert(cursorY + 1, page[cursorY].Substring(cursorX));
                page[cursorY] = page[cursorY].Remove(cursorX);
                cursorY++;
                int oldY = cursorY;
                RedrawLines(cursorY);
                Console.SetCursorPosition(0, oldY);
                break;
            case ConsoleKey.Tab:
                if(page[cursorY].Length + TAB_LENGTH > Console.WindowWidth)
                {
                    page[cursorY] = page[cursorY].Insert(cursorX, new string(' ', TAB_LENGTH));
                    int overflownCharacters = page[cursorY].Length - Console.WindowWidth;
                    int index = page[cursorY].Length - overflownCharacters;
                    page[cursorY + 1] = page[cursorY + 1].Insert(0, page[cursorY].Substring(index));
                    page[cursorY] = page[cursorY].Remove(index);
                    int oldX = cursorX;
                    Console.Write(page[cursorY].Substring(cursorX));
                    oldY = cursorY;
                    RedrawLines(cursorY);

                    if(oldX + TAB_LENGTH >= Console.WindowWidth) 
                    {
                        oldX = oldX + TAB_LENGTH - Console.WindowWidth;
                        oldY++;
                    }
                    else {oldX += TAB_LENGTH;}

                    Console.SetCursorPosition(oldX, oldY);

                }
                else
                {
                    page[cursorY] = page[cursorY].Insert(cursorX, new string(' ', TAB_LENGTH));
                    int oldX = cursorX;
                    Console.Write(page[cursorY].Substring(cursorX));
                    oldX += TAB_LENGTH;
                    cursorX += TAB_LENGTH;
                    Console.SetCursorPosition(oldX, cursorY);
                }
                break;
        }
    }

    public Editor(string path)
    {
        filePath = path;
    }
}
