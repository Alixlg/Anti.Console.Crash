using Application.Tools;

Console.Clear();
Console.Title = "Anti Crash";

while (true)
{
    try
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("<< Select the target program >>");

        var dialog1 = Tools.OpenDialogFile("Select the target program", ".exe");
        string programPath = dialog1.FileName;
        string programName = dialog1.SafeFileName.Replace(".exe", "");

        Console.WriteLine(programName);

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("<< If your program has a Restart.bat file, Selected, if not Cancel it >>");
        var dialog2 = Tools.OpenDialogFile("Select the Restart.bat file if your have", ".bat");
        string batPath = dialog2.FileName;

        Console.ResetColor();
        Console.Title = $"Anti Crash [{programName}]";

        if (string.IsNullOrWhiteSpace(programName) || string.IsNullOrWhiteSpace(programPath))
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(">> Error : Shoma nmitavanid name ya address barname ra ba field khaali por konid !");
            Console.ResetColor();
            continue;
        }

        bool isGreen = true;

        while (true)
        {
            if (!Tools.IsProcessRunning(programName) && !Tools.IsBatRunningFromPath(batPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(">> Program is closed trying to running again . . .");
                Console.ResetColor();
                Tools.StartProcess(programPath);
            }
            else
            {
                Console.ForegroundColor = isGreen ? ConsoleColor.Green : ConsoleColor.Cyan;
                Console.WriteLine(">> Program is running !");
                Console.ResetColor();

                isGreen = !isGreen;
            }

            Tools.KillOneInstance(programName);
            await Task.Delay(2000);
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($">> Error : {ex.Message}");
        Console.ResetColor();
    }
}