using NotatnikAPI.Models;

namespace NotatnikAPI.DTOs
{
    public class ShowNote
    {
        public int Id { get; set; }
        public string Content { get; set; } = "";

        public ShowNote() { }
        public ShowNote(Note note)
        {
            Id = note.Id;
            Content = note.Content;
        }
    }
}
