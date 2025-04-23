using System.Text.Json;

namespace Todo_Manager;

public class TaskManager
{
    private List<Task> _tasks = new List<Task>();

    private static readonly string FilePath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "tasks.json");

    public TaskManager()
    {
        LoadTasks();
    }

    public void AddTask()
    {
        try
        {
            Console.WriteLine("Enter Task Title: ");
            string? title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Invalid title. Title cannot be empty.");

            Console.WriteLine("Enter Task Description: ");
            string? description = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Invalid description. Description cannot be empty.");

            DateTime dueDate;
            Console.WriteLine("Enter Task Due Date (yyyy-MM-dd): ");
            while (!DateTime.TryParse(Console.ReadLine(), out dueDate))
            {
                Console.WriteLine("Invalid date format. Please enter again (yyyy-MM-dd): ");
            }

            if (dueDate < DateTime.Now)
                throw new ArgumentException("Due date cannot be in the past.");

            if (_tasks.Any(t => t.Title!.Equals(title, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Task with this title already exists.");

            _tasks.Add(new Task(title, description, dueDate, false));
            SaveTasks();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding task: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Return to main menu...\n");
        }
    }

    public void ShowTasks()
    {
        try
        {
            if (_tasks.Count == 0)
                throw new TaskNotFoundException("No tasks available to display.\n");

            Console.WriteLine("Tasks:");

            foreach (var task in _tasks.OrderBy(t => t.DueDate))
            {
                task.Display();
                Console.WriteLine();
            }
        }
        catch (TaskNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Return to main menu...\n");
        }
    }

    public void SearchTasks()
    {
        var check = true;
        while (check)
        {
            try
            {
                Console.WriteLine("Enter Task Filter: ");
                Console.WriteLine("1. Show All Tasks");
                Console.WriteLine("2. Show Completed Tasks");
                Console.WriteLine("3. Show Pending Tasks");
                Console.WriteLine("0. Exit Search");

                string? filter = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(filter) || !(new[] { "1", "2", "3","0" }.Contains(filter)))
                    throw new ArgumentException("Invalid filter option. Please enter 1, 2, or 3.");

                switch (filter)
                {
                    case "1":
                        ShowTasks();
                        break;
                    case "2":
                        var completedTasks = _tasks.Where(t => t.IsCompleted).ToList();

                        if (!completedTasks.Any())
                            throw new TaskNotFoundException("No completed tasks found.\n");

                        Console.WriteLine("Completed Tasks:");
                        foreach (var task in completedTasks)
                        {
                            task.Display();
                            Console.WriteLine();
                        }

                        break;
                    case "3":
                        var pendingTasks = _tasks.Where(t => !t.IsCompleted).ToList();

                        if (!pendingTasks.Any())
                            throw new TaskNotFoundException("No pending tasks found.\n");

                        Console.WriteLine("Pending Tasks:");
                        foreach (var task in pendingTasks)
                        {
                            task.Display();
                            Console.WriteLine();
                        }

                        break;
                    case "0":
                        Console.WriteLine("Exiting search...");
                        check = false;
                        break;
                }
                
            }
            catch (TaskNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine();
            }
        }
        Console.WriteLine("Return to main menu...\n");
    }

    public void SetTaskStatus()
    {
        try
        {
            Console.WriteLine("Enter Task Title: ");
            string? inputTitle = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(inputTitle))
                throw new TaskNotFoundException("Invalid title. Title cannot be empty.");
            

            if (!_tasks.Any())
                throw new TaskNotFoundException("No tasks available to update\n");

            var task = _tasks.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.Title) &&
                t.Title.Contains(inputTitle, StringComparison.OrdinalIgnoreCase));

            if (task == null)
                throw new TaskNotFoundException(inputTitle);

            Console.WriteLine($"Current Status: {(task.IsCompleted ? "Done" : "Pending")}");
            Console.WriteLine("Enter New Status: 'Done' or 'Pending'");
            string? status = Console.ReadLine();

            switch (status?.Trim().ToLower())
            {
                case "done":
                    task.IsCompleted = true;
                    SaveTasks();
                    Console.WriteLine("Task marked as completed.\n");
                    break;
                case "pending":
                    task.IsCompleted = false;
                    SaveTasks();
                    Console.WriteLine("Task marked as pending.\n");
                    break;
                default:
                    Console.WriteLine("Invalid status. Please enter 'Done' or 'Pending'.\n");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task status: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Return to main menu...\n");
        }
    }

    public void DeleteTask()
    {
        try
        {
            Console.WriteLine("Enter Task Title: ");
            string? title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Invalid title. Title cannot be empty.");
            if (!_tasks.Any())
                throw new TaskNotFoundException("No tasks available to delete\n");

            var task = _tasks.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.Title) &&
                t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (task == null)
                throw new TaskNotFoundException(title);

            _tasks.Remove(task);
            SaveTasks();
            Console.WriteLine("Task deleted.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Return to main menu...\n");
        }
    }

    public void EditTask()
    {
        try
        {
            Console.WriteLine("Enter Task Title to edit: ");
            string? title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Invalid title. Title cannot be empty.");
            if (!_tasks.Any())
                throw new TaskNotFoundException("No tasks available to edit\n");

            var task = _tasks.FirstOrDefault(t =>
                !string.IsNullOrWhiteSpace(t.Title) &&
                t.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

            if (task == null)
                throw new TaskNotFoundException(title);

            Console.WriteLine($"Editing Task: {task.Title}");

            Console.WriteLine("New Title (leave blank to keep current): ");
            string? newTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTitle)) task.Title = newTitle;

            Console.WriteLine("New Description (leave blank to keep current): ");
            string? newDescription = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newDescription)) task.Description = newDescription;

            Console.Write("Enter new due date (yyyy-MM-dd) or press Enter to skip: ");
            string? newDueDateInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(newDueDateInput))
            {
                if (!DateTime.TryParse(newDueDateInput, out DateTime dueDate))
                    throw new FormatException("Invalid due date format. Please enter a valid date (e.g., 2025-04-22).");

                task.DueDate = dueDate;
            }

            SaveTasks();
            Console.WriteLine("Task Updated\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing task: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Return to main menu...\n");
        }
    }

    private void LoadTasks()
    {
        try
        {
            if (!File.Exists(FilePath)) return;

            var json = File.ReadAllText(FilePath);

            if (string.IsNullOrWhiteSpace(json)) return;

            var loadedTasks = JsonSerializer.Deserialize<List<Task>>(json);

            if (loadedTasks != null)
                _tasks = loadedTasks;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
        }
    }

    public void SaveTasks()
    {
        try
        {
            var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
            Console.WriteLine("Tasks saved\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving tasks: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Return to main menu...\n");
        }
    }
}
