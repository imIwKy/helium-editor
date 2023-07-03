namespace helium_editor;
class FileManager
{
    public string tempFilePath {get; private set;}

    public FileManager()
    {
        if(!File.Exists(Program.filePath))
        {
            StreamWriter file = new StreamWriter(Program.filePath);
            file.Close();
        }

        string tempFileName = $"~{Program.fileName}";
        string tempFilePath = Path.Combine(Path.GetDirectoryName(Program.filePath) ?? "", tempFileName);

        if(File.Exists(tempFilePath)) {File.Delete(tempFilePath);}

        File.Copy(Program.filePath, tempFilePath);
        File.SetAttributes(tempFilePath, FileAttributes.Hidden);
        this.tempFilePath = tempFilePath;
    }

    public List<string> Load()
    {
        StreamReader file = new StreamReader(tempFilePath);
        string? line = file.ReadLine();
        List<string> fileContent = new List<string>();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;

        while(line != null)
        {
            fileContent.Add(line);
            Console.WriteLine(line);
            line = file.ReadLine();
        }

        Console.SetCursorPosition(0,0);
        file.Close();
        return fileContent;
    }

    //Remake to only write the modified lines.
    public void Save(List<string> content, string path)
    {
        StreamWriter file = new StreamWriter(path);
        foreach(string line in content) {file.WriteLine(line);}
        file.Close();
    }
}