using System.Collections.Immutable;

namespace Shared;

public enum QuoteResponseStatusEnum
{
    Cancelled = 0,
    Expired = 1,
    Failed = 2,
    RejectedBadRequest = 3,
    RejectedDuplicate = 4,
    WaitingAcceptance = 5,
    Partial = 6,
    Filled = 7,
    NoInventory = 8,
    AutoAccepted = 9,
    AutoRejected = 10,

    RequestAccepted = 11, //Do we need to acknowledge sender that we accepted request?
    RejectedProhibited = 12,
}

public static class QuoteResponseStatus
{
    public static IImmutableSet<QuoteResponseStatusEnum> FinalizedStatuses { get; } =
        ImmutableHashSet.Create(
            [
                QuoteResponseStatusEnum.Cancelled,
                QuoteResponseStatusEnum.Expired,
                QuoteResponseStatusEnum.Failed,
                QuoteResponseStatusEnum.RejectedBadRequest,
                QuoteResponseStatusEnum.RejectedDuplicate,
                QuoteResponseStatusEnum.Partial,
                QuoteResponseStatusEnum.Filled,
                QuoteResponseStatusEnum.NoInventory,
                QuoteResponseStatusEnum.AutoRejected,
                QuoteResponseStatusEnum.RejectedProhibited,
            ]
        );

    public static IImmutableSet<QuoteResponseStatusEnum> InProgressStatuses { get; } =
        ImmutableHashSet.Create(
            [
                QuoteResponseStatusEnum.WaitingAcceptance,
                QuoteResponseStatusEnum.AutoAccepted,
                QuoteResponseStatusEnum.RequestAccepted,
            ]
        );
}
