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

        file.Close();
    }
}