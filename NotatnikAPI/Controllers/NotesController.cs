using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotatnikAPI.Data;
using NotatnikAPI.Models;
using NotatnikAPI.DTOs;

namespace NotatnikAPI.Controllers
{
    [Authorize]
    [Route("notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowNote>>> GetNotes()
        {
            int userId = GetUserId();

            return await _context.Notes
                .Where(n => n.UserId == userId)
                .Select(n => new ShowNote(n))
                .ToListAsync();
        }

        // GET: notes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowNote>> GetNote(int id)
        {
            int userId = GetUserId();

            var note = await _context.Notes
                .Where(n => n.Id == id && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (note == null)
            {
                return NotFound();
            }

            return new ShowNote(note);
        }

        // POST: notes
        [HttpPost]
        public async Task<ActionResult<ShowNote>> PostNote(CreateNote input)
        {
            var note = new Note
            {
                Content = input.Content,
                UserId = GetUserId(),
            };
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNote", new { id = note.Id }, new ShowNote(note));
        }

        // PUT: notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(int id, CreateNote input)
        {
            if (id != input.Id)
            {
                return BadRequest();
            }

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            if (note.UserId != GetUserId())
            {
                return Forbid();
            }

            note.Content = input.Content;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            if (note.UserId != GetUserId())
            {
                return Forbid();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
