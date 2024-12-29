using Shared.Refit.Reporting;

namespace Shared.Refit;

public interface IReportingApi :
  IInternalInventoryItemController,
  IQuoteDetailsController,
  IReportDataController
{
}
