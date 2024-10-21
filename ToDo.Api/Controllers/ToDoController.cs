using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Api.Data;
using ToDo.Api.DTOs;
using ToDo.Api.Models;

namespace ToDo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToDoController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET all todo items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoDto>>> GetToDos()
        {
            if (_context.ToDos == null)
            {
                return NotFound(new ErrorResponse("No to do items found"));
            }
            return await _context.ToDos.Select(t=> new ToDoDto(t)).ToListAsync();
        }

        /// <summary>
        /// GET single todo item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoDto>> GetToDoItem(int id)
        {
            if (_context.ToDos == null)
            {
                return NotFound(new ErrorResponse("No to do items found"));
            }
            var toDoItem = await _context.ToDos.FindAsync(id);

            if (toDoItem == null)
            {
                return NotFound(new ErrorResponse($"No to do items found by id {id}"));
            }

            return new ToDoDto(toDoItem);
        }

        /// <summary>
        /// UPDATE existing todo item by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="toDoItem"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoItem(int id, ToDoDto toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return BadRequest(new ErrorResponse("Submitted to do item id doesn't match request id"));
            }

            var item = await _context.ToDos.FirstOrDefaultAsync(t => t.Id == id);

            if (item == null)
            {
                return NotFound(new ErrorResponse($"No to do items found by id {id}"));
            }

            item.UserId = toDoItem.UserId;
            item.Type = toDoItem.Type;
            item.EndDate = toDoItem.EndDate;
            item.Content = toDoItem.Content;

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoItemExists(id))
                {
                    return NotFound(new ErrorResponse($"No to do items found by id {id}"));
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// CREATE new todo item
        /// </summary>
        /// <param name="toDoItem"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ToDoDto>> PostToDoItem(ToDoForCreateDto toDoItem)
        {
            if (_context.ToDos == null)
            {
                return StatusCode(500, $"Server error - database issue");
            }

            var toDoEntity = new ToDoItem()
            {
                Content = toDoItem.Content,
                EndDate = toDoItem.EndDate,
                Type = toDoItem.Type,
                UserId = toDoItem.UserId
            };
            _context.ToDos.Add(toDoEntity);
            await _context.SaveChangesAsync();

            var createdTodo = new ToDoDto(toDoEntity);

            return CreatedAtAction("GetToDoItem", new { id = createdTodo.Id }, createdTodo);
        }

        /// <summary>
        /// DELETE todo item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(int id)
        {
            if (_context.ToDos == null)
            {
                return StatusCode(500, $"Server error - database issue");
            }
            var toDoItem = await _context.ToDos.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound(new ErrorResponse($"No to do items found by id {id}"));
            }

            _context.ToDos.Remove(toDoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoItemExists(int id)
        {
            return (_context.ToDos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
