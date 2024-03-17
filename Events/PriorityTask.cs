namespace Events
{
    public class PriorityTask
    {
        public int TaskID { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; } // value between 1 and 5

        public PriorityTask(int taskID, string? description, int priority)
        {
            TaskID = taskID;
            Description = description;
            Priority = priority;
        }
    }
}
