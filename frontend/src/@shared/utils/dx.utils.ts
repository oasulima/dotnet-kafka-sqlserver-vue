import { TimeEnum } from '@/constants';
import type { NativeEventInfo } from 'devextreme/events';
import type dxDataGrid from 'devextreme/ui/data_grid';
import type dxDateBox from 'devextreme/ui/date_box';
import type dxNumberBox from 'devextreme/ui/number_box';
import type dxTextBox from 'devextreme/ui/text_box';

export const uppercaseTextBoxStyle = { 'style': 'text-transform: uppercase' };

export function applyDotOnKeyDown(e: NativeEventInfo<dxNumberBox, KeyboardEvent>) {
  const text = e.component.option('text');
  const key = (e.event as any)?.originalEvent?.key;

  if (!text && key === '.') {
    e.component.option('value', 0);
  }
}

export function textBoxValueUpper(params: { component: dxTextBox, value: string | undefined }) {
  params.component.option('value', params.value?.toUpperCase());
}

export interface DxGridColumnDisplayOptions {
  sortIndex?: number;
  sortOrder?: 'asc' | 'desc';
  filterValues?: any[];
  width: number | string;
}

export function applyDxGridExactColumnFilter(component: dxDataGrid | undefined, column: string, value: string | number | undefined) {
  if (!component) {
    return;
  }

  component.columnOption(column, 'filterOperations', ['=']);
  component.columnOption(column, 'filterValue', value);
}


export function setMaxDateAsToday(e: { component: dxDateBox }) {
  const msInDay = TimeEnum._24hours;
  const maxNoOffset = ((Math.floor(Date.now() / msInDay)) + 1) * msInDay - 1;
  const max = maxNoOffset + new Date(maxNoOffset).getTimezoneOffset() * 60 * 1000;
  const oldMax = e.component.option('max');

  if (oldMax !== max) {
    e.component.option('max', max);
  }
}
