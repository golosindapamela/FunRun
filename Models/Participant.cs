namespace Fun_Run_Registration.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public string Distance { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string TShirtSize { get; set; }
        public string? DogsName { get; set; }
        public string? DogsBreed { get; set; }
        public int? DogsAge { get; set; }
        public string Payment { get; set; }

    }
}
