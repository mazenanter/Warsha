using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.UseVoucher
{
    public record UseVoucherCommand(string VoucherCode, int BookingId) : IRequest<Result>;
}
