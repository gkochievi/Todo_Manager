namespace Todo_Manager;

public class Task(string? title, string? description, DateTime dueDate, bool isCompleted)
{
    private Guid Id { get; set; } = Guid.NewGuid();
    public string? Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public DateTime DueDate { get; set; } = dueDate;
    public bool IsCompleted { get; set; } = isCompleted;

    public void Display()
    {
        Console.WriteLine($"ID: {Id} | Title: {Title} | Due Date: {DueDate.ToShortDateString()} | Completed: {IsCompleted}");
        Console.WriteLine($"Description: {Description}");

    }
}