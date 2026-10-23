using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data
{
    public static class SeedData
    {
        public static List<Project> Projects()
        {
            return new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Name = "Website Redesign",
                    Description = "Refresh the company website with a modern look.",
                    CreatedAt = new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc)
                },
                new Project
                {
                    Id = 2,
                    Name = "Mobile App Launch",
                    Description = "Prepare and release version 1.0 of the mobile app.",
                    CreatedAt = new DateTime(2026, 10, 5, 9, 0, 0, DateTimeKind.Utc)
                }
            };
        }

        public static List<TaskItem> Tasks()
        {
            return new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    ProjectId = 1,
                    Title = "Create wireframes",
                    Description = "Sketch the home, about and contact pages.",
                    Status = TaskItemStatus.Done,
                    DueDate = new DateTime(2026, 10, 10),
                    CreatedAt = new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new TaskItem
                {
                    Id = 2,
                    ProjectId = 1,
                    Title = "Design homepage mockup",
                    Status = TaskItemStatus.InProgress,
                    DueDate = new DateTime(2026, 10, 28),
                    CreatedAt = new DateTime(2026, 10, 2, 10, 0, 0, DateTimeKind.Utc)
                },
                new TaskItem
                {
                    Id = 3,
                    ProjectId = 1,
                    Title = "Build contact form",
                    Status = TaskItemStatus.Todo,
                    DueDate = new DateTime(2026, 11, 5),
                    CreatedAt = new DateTime(2026, 10, 3, 10, 0, 0, DateTimeKind.Utc)
                },
                new TaskItem
                {
                    Id = 4,
                    ProjectId = 2,
                    Title = "Write app store description",
                    Status = TaskItemStatus.InProgress,
                    DueDate = new DateTime(2026, 11, 15),
                    CreatedAt = new DateTime(2026, 10, 5, 11, 0, 0, DateTimeKind.Utc)
                },
                new TaskItem
                {
                    Id = 5,
                    ProjectId = 2,
                    Title = "Set up push notifications",
                    Description = "Send a reminder when a task is due.",
                    Status = TaskItemStatus.Todo,
                    CreatedAt = new DateTime(2026, 10, 6, 11, 0, 0, DateTimeKind.Utc)
                }
            };
        }
    }
}
