import { uniq } from 'lodash-es';
import type { QuoteSourceInfo } from '@/lib/api/v1';

export function formatQuoteSourceInfos(sourceInfos: QuoteSourceInfo[]): string[] {
  const formattedSources = sourceInfos.map((sourceInfo) => {
    const sourceUp = sourceInfo.source!.toUpperCase();
    const providerUp = sourceInfo.provider?.toUpperCase() ?? '';

    if (providerUp && providerUp !== sourceUp) {
      return `${sourceUp}-${providerUp}`;
    }

    return sourceUp;
  });

  return uniq(formattedSources);
}