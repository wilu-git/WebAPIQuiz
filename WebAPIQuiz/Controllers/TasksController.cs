using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIQuiz.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private static List<Models.TaskItem> tasks = new List<Models.TaskItem>
            {
                new Models.TaskItem { Id = 1, Title = "Task 1", IsCompleted = false },
                new Models.TaskItem { Id = 2, Title = "Task 2", IsCompleted = true },
                new Models.TaskItem { Id = 3, Title = "Task 3", IsCompleted = false }
            };

        [HttpGet("tasks")]
        public IActionResult GetTasks()
        {
            return Ok(tasks);
        }

        [HttpPost("tasks")]
        public IActionResult CreateTask([FromBody] Models.TaskItem task)
        {
            return Ok("Task Created");
        }

        [HttpGet("tasks/{id}")]
        public IActionResult GetTaskById(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPut("tasks/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateTask(int id, [FromBody] Models.TaskItem updatedTask)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            task.Title = updatedTask.Title;
            task.IsCompleted = updatedTask.IsCompleted;
            return NoContent();
        }

        [HttpDelete("tasks/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteTask(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            tasks.Remove(task);
            return NoContent();
        }
    }
}
