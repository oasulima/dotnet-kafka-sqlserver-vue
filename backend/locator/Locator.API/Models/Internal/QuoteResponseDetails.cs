using Locator.API.Services.Interfaces;
using Shared;

namespace Locator.API.Models.Internal;

internal record QuoteResponseDetails(
    PriceCalculationResult? PriceCalculationResult = null,
    QuoteRequest? QuoteRequest = null,
    QuoteResponseStatusEnum? Status = null,
    string? Message = null,
    Quote? Quote = null
);
