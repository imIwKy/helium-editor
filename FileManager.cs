namespace helium_editor;
class FileManager
{
    public List<string> Load(string name, string path)
    {

        if(!File.Exists(path))
        {
            StreamWriter writer = new StreamWriter(path);
            writer.Close();
        }

        string tempFileName = $"~{name}";
        string tempFilePath = Path.Combine(Path.GetDirectoryName(path) ?? "", tempFileName);

        if(File.Exists(tempFilePath)) {File.Delete(tempFilePath);}

        File.Copy(path, tempFilePath);
        File.SetAttributes(tempFilePath, FileAttributes.Hidden);
        StreamReader reader = new StreamReader(tempFilePath);
        string? line = reader.ReadLine();
        List<string> fileContent = new List<string>();

        while(line != null)
        {
            fileContent.Add(line);
            line = reader.ReadLine();
        }

        reader.Close();
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