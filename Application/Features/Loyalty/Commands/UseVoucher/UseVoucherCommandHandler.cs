using Application.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Loyalty.Commands.UseVoucher
{
    public class UseVoucherCommandHandler : IRequestHandler<UseVoucherCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UseVoucherCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Result> Handle(UseVoucherCommand command, CancellationToken ct)
        {
            var voucher = await _unitOfWork.RewardVouchers
                .GetByCodeAsync(command.VoucherCode, ct);

            if (voucher is null)
                return Result.Failure("Voucher not found");

            voucher.Use(command.BookingId);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Success("Voucher used successfully");
        }
    }
}
