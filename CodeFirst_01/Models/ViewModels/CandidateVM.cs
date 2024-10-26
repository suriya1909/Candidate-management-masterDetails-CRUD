namespace CodeFirst_01.Models.ViewModels
{
    public class CandidateVM
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = default!;
        public IFormFile ?ImagePath { get; set; }
        public string? Image { get; set; }
        public bool Fresher { get; set; }
        public List<int> SkillList { get; set; } = new List<int>();
    }
}
