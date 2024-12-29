import CustomStore from 'devextreme/data/custom_store';

export function BuildLoadOnlyStore(load: () => Promise<unknown[]>, key = 'id') {
  return new CustomStore({
    key,
    load
  });
}
