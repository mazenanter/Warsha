using Domain.Common;
using MediatR;

namespace Application.Features.Workshop.Commands.Services.AddWorkshopService
{
    public class AddWorkshopServiceCommand : IRequest<Result>
    {
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int ServiceCategoryId { get; set; }
        public int Duration { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionAr { get; set; }
    }
}
