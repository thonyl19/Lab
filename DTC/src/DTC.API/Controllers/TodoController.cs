using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DTC.API.Data;
using DTC.API.Models;

namespace DTC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ApplicationDbContext context, ILogger<TodoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 取得所有待辦事項
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
        {
            try
            {
                var todos = await _context.Todos.ToListAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all todos");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 根據 ID 取得待辦事項
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Todo>> GetById(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }
                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting todo {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 建立新的待辦事項
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Todo>> Create(Todo todo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                todo.CreatedAt = DateTime.UtcNow;
                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating todo");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 更新待辦事項
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Todo todo)
        {
            try
            {
                if (id != todo.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingTodo = await _context.Todos.FindAsync(id);
                if (existingTodo == null)
                {
                    return NotFound();
                }

                existingTodo.Title = todo.Title;
                existingTodo.Description = todo.Description;
                existingTodo.IsCompleted = todo.IsCompleted;
                existingTodo.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating todo {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 刪除待辦事項
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }

                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting todo {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 