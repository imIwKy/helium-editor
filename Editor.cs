namespace helium_editor;
class Editor
{
    private List<string> page = new List<string>();

    public void DisplayContent(string path)
    {
        StreamReader file = new StreamReader(path);

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
        Console.SetCursorPosition(0,0);
    }

    public void Edit(string path)
    {
        
    }
}