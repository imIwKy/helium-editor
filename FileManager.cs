namespace helium_editor;
class FileManager
{
    public static List<string> Load(string name, string path)
    {

        if(!File.Exists(path))
        {
            StreamWriter writer = new(path);
            writer.Close();
        }

        string tempFilePath = Path.Combine(Path.GetDirectoryName(path) ?? "", $"~{name}");

        if(File.Exists(tempFilePath)) {File.Delete(tempFilePath);}

        File.Copy(path, tempFilePath);
        File.SetAttributes(tempFilePath, FileAttributes.Hidden);
        StreamReader reader = new(tempFilePath);
        string? line = reader.ReadLine();
        List<string> fileContent = [];

        while(line != null)
        {
            fileContent.Add(line);
            line = reader.ReadLine();
        }

        reader.Close();
        return fileContent;
    }

    //Remake to only write the modified lines.
    public static void Save(List<string> content, string path)
    {
        StreamWriter file = new(path);
        foreach(string line in content) {file.WriteLine(line);}
        file.Close();
    }
}