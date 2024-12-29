import type { QuoteResponseStatusEnum } from '@/lib/api/v1';

const hiddenStatuses: QuoteResponseStatusEnum[] = [
  'RequestAccepted',
  'Expired',
  'Failed',
  'WaitingAcceptance',
  'AutoAccepted'
];

export const ReportEligibleLocateStatuses: QuoteResponseStatusEnum[] =
  [
    'AutoRejected',
    'Cancelled',
    'Filled',
    'NoInventory',
    'Partial',
    'RejectedBadRequest',
    'RejectedDuplicate',
    'RejectedProhibited'
  ];
