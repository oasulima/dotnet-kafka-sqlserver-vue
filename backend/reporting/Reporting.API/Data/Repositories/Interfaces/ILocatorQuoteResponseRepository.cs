using Reporting.API.Data.Models.DbModels;
using Reporting.API.Data.Models.DbParams;
using Shared;

namespace Reporting.API.Data.Repositories.Interfaces;

public interface ILocatorQuoteResponseRepository
{
  LocatorQuoteResponseDb[] GetLocatorQuoteResponses(DateTime from, DateTime to, int take, int skip);
  void Add(LocatorQuoteResponseDb model);
  LocatesReportDataDb[] GetLocatesReportData(GetLocatesReportDataDbParams model);
}