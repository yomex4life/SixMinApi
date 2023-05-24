namespace SixMinApi.DTOs
{
    public class CommandReadDto
    {
        public int Id { get; set; }
        public string? HowTo { get; set; }  //question mark means nullable
        public string? Platform { get; set; }
        public string? CommandLine { get; set; } 
    }
}