namespace ForLab.DTO.Common
{
    public class BaseFilterDto
    {
        public bool ApplySort { get; set; }
        public string SortProperty { get; set; }
        public bool IsAscending { get; set; }
        public bool? IsActive { get; set; }
        // For security
        public int LoggedInUserId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}
