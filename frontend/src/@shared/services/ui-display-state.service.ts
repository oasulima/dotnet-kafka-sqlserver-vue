import { localStorageService } from '@/shared/services/local-storage.service';

class UiDisplayStateService {
  private state = {} as Record<string, any>;

  constructor() {
    this.load();
  }

  public loadProp<T>(propName: string): T | undefined {
    return this.state[propName];
  }

  public saveProp<T>(propName: string, propValue: T) {
    this.state[propName] = propValue;
    this.save();
  }

  private save() {
    localStorageService.set('UI_State', this.state);
  }

  private load() {
    this.state = localStorageService.get('UI_State') || {};
  }
}

export const uiDiplayStateService = new UiDisplayStateService();