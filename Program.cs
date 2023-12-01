// See https://aka.ms/new-console-template for more information

bool couldParse = false;
IDayCommand command = new NullDay();
int day = -1;

if(args.Length > 0)
{
    couldParse = int.TryParse(args[0], out day);    
    if(couldParse) {
        command = new DayCommandFactory().GetCommand(day);
        Console.WriteLine(command.Execute());        
    }    
}
else
{
    do {
        Console.Write("Enter the day you want to execute: ");
        couldParse = int.TryParse(Console.ReadLine(), out day);
        if(couldParse) {
            command = new DayCommandFactory().GetCommand(day);
        }
    } while (!couldParse);

    Console.WriteLine(command.Execute());
}
