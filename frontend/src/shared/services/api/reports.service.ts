import { client } from '../http.service';
import type { LocatesReportData, LocatesReportDataRequest } from '@/lib/api/v1';
import { toResult, type Result } from '@/constants';

export class ReportsService {

  public async getLocatesReportData(
    params: LocatesReportDataRequest
  ): Promise<Result<LocatesReportData[]>> {
    const { data } = await client.POST('/api/report/locates',
      {
        body: params
      }
    );
    return toResult(data);
  }
}

export const reportsService = new ReportsService();