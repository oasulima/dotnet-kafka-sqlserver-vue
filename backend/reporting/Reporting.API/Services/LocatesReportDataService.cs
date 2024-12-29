using System;
using System.Collections.Generic;
using System.Linq;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.Services.Interfaces;
using Reporting.API.Utility;
using Shared.Quote;
using Shared.Reporting;
using Shared.Reporting.Requests;

namespace Reporting.API.Services;

public class LocatesReportDataService : ILocatesReportDataService
{
    private readonly ILocatorQuoteResponseRepository _locatorQuoteResponseRepository;

    public LocatesReportDataService(ILocatorQuoteResponseRepository locatorQuoteResponseRepository)
    {
        _locatorQuoteResponseRepository = locatorQuoteResponseRepository;
    }

    public IList<LocatesReportData> GetLocatesReportData(LocatesReportDataRequest request)
    {
        var dbParams = request.ToLocatesReportDataDbParams();
        var data = _locatorQuoteResponseRepository.GetLocatesReportData(dbParams);
        return data.ToLocatesReportData();
    }

    public IList<LocatorQuoteResponse> GetLocatorQuoteResponses(DateTime from, DateTime to, int take, int skip)
    {
        var quoteResponses = _locatorQuoteResponseRepository.GetLocatorQuoteResponses(from, to, take, skip);
        return quoteResponses.Select(Mappers.ToLocatorQuoteResponse).ToArray();
    }

    public void AddLocatorQuoteResponse(LocatorQuoteResponse locatorQuoteResponse)
    {
        _locatorQuoteResponseRepository.Add(locatorQuoteResponse.ToLocatorQuoteResponseDb());
    }
}