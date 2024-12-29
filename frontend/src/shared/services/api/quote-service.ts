import { client } from '../http.service';
import type { QuoteRequest } from '@/lib/api/v1';

export class QuoteService {

  public async quote(request: QuoteRequest): Promise<void> {
    await client.POST('/api/quote',
      {
        body: request
      }
    );
  }
}

export const quoteService = new QuoteService();