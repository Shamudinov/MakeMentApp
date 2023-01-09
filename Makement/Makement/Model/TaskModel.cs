using Makement.Enum;

namespace Makement.Model
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public TaskStatusEnum Status { get; set; }
    }
}
