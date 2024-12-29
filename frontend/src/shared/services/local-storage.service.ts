class LocalStorageService {
  public get<T>(key: string) {
    return JSON.parse(localStorage.getItem(key) || 'null') as T | null;
  }

  public set<T>(key: string, item: T) {
    localStorage.setItem(key, JSON.stringify(item));
  }
}

export const localStorageService = new LocalStorageService();
