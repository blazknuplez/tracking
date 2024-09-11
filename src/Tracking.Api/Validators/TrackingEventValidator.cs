using FluentValidation;
using Tracking.Ef;
using Tracking.Models;

namespace Tracking.Validators;

internal class TrackingEventValidator : AbstractValidator<TrackingEventModel>
{
    private readonly TrackingDbContext _context;
    
    public TrackingEventValidator(TrackingDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.AccountId).Must(IsValidAccount).WithMessage($"Invalid {nameof(TrackingEventModel.AccountId)} - account must exist and be active");
        RuleFor(x => x.Data).NotEmpty().WithMessage("Missing event data");
    }

    private bool IsValidAccount(long id)
    {
        return _context.Accounts.Any(x => x.Id == id && x.IsActive);
    }
}