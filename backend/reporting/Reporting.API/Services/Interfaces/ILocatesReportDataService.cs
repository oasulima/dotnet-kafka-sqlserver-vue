using System;
using System.Collections.Generic;
using Shared.Quote;
using Shared.Reporting;
using Shared.Reporting.Requests;

namespace Reporting.API.Services.Interfaces;

public interface ILocatesReportDataService
{
    IList<LocatesReportData> GetLocatesReportData(LocatesReportDataRequest request);

    IList<LocatorQuoteResponse> GetLocatorQuoteResponses(DateTime from, DateTime to, int take, int skip);

    void AddLocatorQuoteResponse(LocatorQuoteResponse locatorQuoteResponse);
}