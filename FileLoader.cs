namespace helium_editor;
class FileLoader
{
    public string Load(string path, string name)
    {
        if(!File.Exists(path))
        {
            StreamWriter file = File.CreateText(path);
            file.Close();
        }

        string tempFileName = $"~{name}";
        string tempFilePath = Path.Combine(Path.GetDirectoryName(path) ?? "", tempFileName);

        File.Copy(path, tempFilePath);
        File.SetAttributes(tempFilePath, FileAttributes.Hidden);

        return tempFilePath;
    }
}