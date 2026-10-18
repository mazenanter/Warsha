namespace Application.Features.Admin.DTOs
{
    public class AdminClientListItemDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int CarsCount { get; set; }
        public int BookingsCount { get; set; }
        public decimal TotalSpending { get; set; }
        public string Status { get; set; } = default!; // "active" | "suspended"
        public DateTime CreatedAt { get; set; }
    }

    public class AdminClientCountsDto
    {
        public int All { get; set; }
        public int Active { get; set; }
        public int Suspended { get; set; }
        public int New { get; set; }
    }

    public class AdminClientListResponseDto
    {
        public List<AdminClientListItemDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public AdminClientCountsDto Counts { get; set; } = new();
    }
}