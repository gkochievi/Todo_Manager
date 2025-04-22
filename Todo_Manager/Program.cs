namespace Todo_Manager
{
    internal abstract class Program
    {
        static void Main()
        {
            var manager = new TaskManager();

            while (true)
            {
                Console.WriteLine("=== Todo Manager ===");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. Show Tasks");
                Console.WriteLine("3. Search Tasks");
                Console.WriteLine("4. Update Task Status");
                Console.WriteLine("5. Delete Task");
                Console.WriteLine("6. Edit Task");
                Console.WriteLine("0. Exit");
                
                var input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    
                    case "1":
                        manager.AddTask();
                        break;
                    case "2":
                        manager.ShowTasks();
                        break;
                    case "3":
                        manager.SearchTasks();
                        break;
                    case "4":
                        manager.SetTaskStatus();
                        break;
                    case "5":
                        manager.DeleteTask();
                        break;
                    case "6":
                        manager.EditTask();
                        break;
                    case "0":
                        manager.SaveTasks();
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        break;
                    
                }
            }

        }
    }
}