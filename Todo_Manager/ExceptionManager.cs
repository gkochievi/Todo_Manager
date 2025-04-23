namespace Todo_Manager;

public class TaskNotFoundException(string title) : Exception($"Task with title '{title}' not found.");