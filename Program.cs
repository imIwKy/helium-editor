namespace helium_editor;
class Program
{
    private static Editor? editor;
    public static string? filePath {get; private set;}
    public static string? fileName {get; private set;}

    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("No file was provided exiting...");
            Environment.Exit(160);
        }

        filePath = args[0];
        fileName = Path.GetFileName(filePath);

        if(fileName.Length == 0)
        {
            Console.WriteLine("Invalid path exiting...");
            Environment.Exit(160);
        }

        editor = new Editor();
    }
}
