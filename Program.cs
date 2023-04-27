namespace helium_editor;
class Program
{
    private static FileManager? fileManager;
    private static Editor? editor;
    private static string? filePath;
    private static string? tempFilePath;

    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("No file was provided exiting...");
            Environment.Exit(160);
        }

        fileManager = new FileManager();
        editor = new Editor();

        filePath = args[0];
        {
            string fileName = Path.GetFileName(filePath);

            if(fileName.Length == 0)
            {
                Console.WriteLine("Invalid path exiting...");
                Environment.Exit(160);
            }

           tempFilePath = fileManager.Load(filePath, fileName);
        }

        editor.DisplayContent(tempFilePath);
        File.Delete(tempFilePath);
    }
}
