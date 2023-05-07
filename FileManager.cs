namespace helium_editor;
class FileManager
{
    public string Load(string path, string name)
    {
        if(!File.Exists(path))
        {
            StreamWriter file = new StreamWriter(path);
            file.Close();
        }

        string tempFileName = $"~{name}";
        string tempFilePath = Path.Combine(Path.GetDirectoryName(path) ?? "", tempFileName);

        //Delete the temporary file so it doesnt cause an exception if its there from a previous crash.
        if(File.Exists(tempFilePath)) {File.Delete(tempFilePath);}

        File.Copy(path, tempFilePath);
        File.SetAttributes(tempFilePath, FileAttributes.Hidden);

        return tempFilePath;
    }

    //Remake to only write the modified lines.
    public void Save(List<string> content, string path)
    {
        StreamWriter file = new StreamWriter(path);
        foreach(string line in content) {file.WriteLine(line);}
        file.Close();
    }
}