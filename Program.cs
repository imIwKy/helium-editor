namespace helium_editor;
class Program
{
    private static Editor? editor;
    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("No file was provided exiting...");
            Environment.Exit(160);
        }

        string filePath = args[0];
        string fileName = Path.GetFileName(filePath);

        if(fileName.Length == 0)
        {
            Console.WriteLine("Invalid path exiting...");
            Environment.Exit(160);
        }

        FileManager fileManager = new FileManager();
        editor = new Editor(fileManager.Load(fileName, filePath), filePath);
    }
}