namespace helium_editor;
class Editor
{
    private static FileLoader? loader;
    //private static FileSaver saver;
    private static string? filePath;
    private static string? tempFilePath;

    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("No file was provided exiting...");
            Environment.Exit(160);
        }

        loader = new FileLoader();
        //saver = new FileSaver();

        filePath = args[0];
        {
            string fileName = Path.GetFileName(filePath);

            if(fileName.Length == 0)
            {
                Console.WriteLine("Invalid path exiting...");
                Environment.Exit(160);
            }

           tempFilePath = loader.Load(filePath, fileName);
        }
        
        File.Delete(tempFilePath);
    }
}
