using FluentValidation;
using Tracking.Ef;
using Tracking.Requests;

namespace Tracking.Validators;

internal class PostEventRequestValidator : AbstractValidator<PostEventRequest>
{
    private readonly TrackingDbContext _context;
    
    public PostEventRequestValidator(TrackingDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.AccountId).Must(IsValidAccount).WithMessage($"Invalid {nameof(PostEventRequest.AccountId)} - account must exist and be active");
        RuleFor(x => x.Body).NotNull().WithMessage("Missing request body");
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        RuleFor(x => x.Body.Data).NotEmpty().WithMessage("Missing event data").When(x => x.Body != null);
    }

    private bool IsValidAccount(long id)
    {
        return _context.Accounts.Any(x => x.Id == id && x.IsActive);
    }
}