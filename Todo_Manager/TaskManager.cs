using System.Text.Json;

namespace Todo_Manager;

public class TaskManager
{
    private List<Task>? _tasks = new List<Task>();
    
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "tasks.json");
    
    public TaskManager()
    {
        LoadTasks();
    }

    
    
    public void AddTask()
    {
        Console.WriteLine("Enter Task Title: ");
        string? title = Console.ReadLine();
        Console.WriteLine("Enter Task Description: ");
        string? description = Console.ReadLine();
        Console.WriteLine("Enter Task Due Date (yyyy-mm-dd): ");
        DateTime dueDate;
        while (!DateTime.TryParse(Console.ReadLine(), out dueDate))
        {
            Console.WriteLine("Invalid date format. Please enter again (yyyy-mm-dd): ");
        }
        
        _tasks?.Add(new Task(title, description, dueDate, false));
        SaveTasks();
    }

    
    
    public void ShowTasks()
    {
        if (_tasks is { Count: 0 })
        {
            Console.WriteLine("No tasks to show.\n");
            return;
        }
        Console.WriteLine("Tasks:");
        if (_tasks != null)
            foreach (var task in _tasks.OrderBy(t => t.DueDate))
            {
                task.Display();
                Console.WriteLine();
            }
    }

    
    
    public void SearchTasks()
    {
        Console.WriteLine("Enter Task Filter: ");
        Console.WriteLine("1. Show All Tasks");
        Console.WriteLine("2. Show Completed Tasks");
        Console.WriteLine("3. Show Pending Tasks");
        
        string? filter = Console.ReadLine();
        
        if (_tasks != null)
        {
            switch (filter)
            {
                case "1":
                    ShowTasks();
                    break;
                case "2":
                    var completedTasks = _tasks.Where(t => t.IsCompleted).ToList();
                    if (completedTasks.Count == 0)
                    {
                        Console.WriteLine("No completed tasks.\n");
                        return;
                    }
                    Console.WriteLine("Completed Tasks:");
                    foreach (var task in completedTasks)
                    {
                        task.Display();
                        Console.WriteLine();
                    }
                    break;
                case "3":
                    var pendingTasks = _tasks.Where(t => !t.IsCompleted).ToList();
                    if (pendingTasks.Count == 0)
                    {
                        Console.WriteLine("No pending tasks.\n");
                        return;
                    }
                    Console.WriteLine("Pending Tasks:");
                    foreach (var task in pendingTasks)
                    {
                        task.Display();
                        Console.WriteLine();
                    }
                    break;
                default:
                    Console.WriteLine("Invalid filter option.\n");
                    break;
            }
        }
    }

    
    
    public void SetTaskStatus()
    {
        Console.WriteLine("Enter Task Title: ");
        string? inputTitle = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(inputTitle))
        {
            Console.WriteLine("Invalid title.\n");
            return;
        }

        if (_tasks != null)
        {
            var task = _tasks.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.Title) &&
                t.Title!.ToLower().Contains(inputTitle.ToLower()));

            if (task == null)
            {
                Console.WriteLine("Task not found.\n");
                return;
            }

            Console.WriteLine($"Current Status: {(task.IsCompleted ? "Done" : "Pending")}");
            Console.WriteLine("Enter New Status: 'Done' or 'Pending'");
            string? status = Console.ReadLine();

            if (status?.ToLower() == "done")
            {
                task.IsCompleted = true;
                SaveTasks();
                Console.WriteLine("Task marked as completed.\n");
            }
            else if (status?.ToLower() == "pending")
            {
                task.IsCompleted = false;
                SaveTasks();
                Console.WriteLine("Task marked as pending.\n");
            }
            else
            {
                Console.WriteLine("Invalid status. Please enter 'Done' or 'Pending'.\n");
            }
        }
        else
        {
            Console.WriteLine("No tasks to update.\n");
        }
    }

    
    
    
    public void DeleteTask()
    {
        Console.WriteLine("Enter Task Title: ");
        string? title = Console.ReadLine();

        if (_tasks != null)
        {
            var task = _tasks.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.Title) &&
                t.Title!.ToLower().Contains(title?.ToLower() ?? string.Empty));

            if (task == null)
            {
                Console.WriteLine("Task not found.\n");
                return;
            }
        
            _tasks.Remove(task);
            SaveTasks();
            Console.WriteLine("Task deleted.\n");
        }
        else
        {
            Console.WriteLine("No tasks to delete.\n");
        }
    }



    public void EditTask()
    {
        Console.WriteLine("Enter Task Title to edit: ");
        string? title = Console.ReadLine();
        
        if(_tasks == null || _tasks.Count == 0)
        {
            Console.WriteLine("No tasks to edit.\n");
            return;
        }
        
        var task = _tasks.FirstOrDefault(t =>
            !string.IsNullOrWhiteSpace(t.Title) &&
            t.Title!.ToLower().Contains(title?.ToLower() ?? string.Empty));
        
        if (task == null)
        {
            Console.WriteLine("Task not found.\n");
            return;
        }

        Console.WriteLine($"Editing Task: {task.Title}");
        
        Console.WriteLine("New Title (leave blank to keep current): ");
        string? newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle)) task.Title = newTitle;
        
        Console.WriteLine("New Description (leave blank to keep current): ");
        string? newDescription = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDescription)) task.Description = newDescription;
        
        Console.WriteLine("New Due Date (leave blank to keep current): ");
        string? newDueDateInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDueDateInput) && DateTime.TryParse(newDueDateInput, out DateTime newDueDate))
        {
            task.DueDate = newDueDate;
        }
        else
        {
            Console.WriteLine("Invalid date format. Keeping current due date.");
        }

        SaveTasks();
        Console.WriteLine("Task Updated\n");
    }
    
    
    
    private void LoadTasks()
    {
        if(!File.Exists(FilePath)) return;
        
        var json = File.ReadAllText(FilePath);
        var loadedTasks = JsonSerializer.Deserialize<List<Task>>(json);

        if (loadedTasks == null)
        {
            Console.WriteLine("No tasks loaded\n");
        }
        _tasks = loadedTasks;
        Console.WriteLine("Tasks loaded\n");
    }
    
    
    public void SaveTasks()
    {
        var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
        Console.WriteLine("Tasks saved\n");
    }
    
}