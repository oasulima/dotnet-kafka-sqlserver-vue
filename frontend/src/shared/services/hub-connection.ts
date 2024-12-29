import { TimeEnum } from '@/constants';
import notify from 'devextreme/ui/notify';
import type { Subscription } from 'rxjs';
import { authService, type AuthService } from './auth.service';

type Listener = { methodName: string, callback: (...args: any[]) => void };

function BuildConnection(listeners: Listener[]) {
  const connection = new EventSource(import.meta.env.VITE_ADMINUI_BASE_URL + 'sse');

  for (const { methodName, callback } of listeners) {
    connection.addEventListener(methodName, (event) => callback(JSON.parse(event.data)));
  }

  window.addEventListener('beforeunload', () => {
    console.log('connection.close()');
    connection.close();
  });

  return connection;
}

class HubConnectionService {
  private listeners: Listener[] = [];
  // private authSubscription: Subscription | null = null;
  private interval: number | null = null;
  private hasError?: boolean;
  private connection: EventSource | null = null;


  constructor(
    // private authService: AuthService,
    healthCheckIntervalInMilliseconds: number
  ) {
    this.interval = setInterval(() => this.handleInterval(), healthCheckIntervalInMilliseconds);
    // this.authSubscription = this.authService.getToken$().subscribe((token) => this.handleTokenUpdate(token));
    this.connection = BuildConnection(this.listeners);
    // this.connection.start();
  }

  public addListner(methodName: string, callback: (...args: any[]) => void) {
    this.listeners.push({ methodName, callback });
    this.connection?.addEventListener(methodName, (event) => {
      return callback(JSON.parse(event.data));
    });
  }

  // private handleTokenUpdate(token: string | null) {
  //   this.dropConnection();

  //   if (token) {
  //     this.connection = BuildConnection(token, this.listeners);
  //     this.connection.start();
  //   }
  // }

  private dropConnection() {
    if (!this.connection) {
      return;
    }

    // this.connection.stop();
    this.connection = null;
  }

  private handleInterval() {
    if (!this.connection) {
      this.hasError = false;
      return;
    }

    // if ([HubConnectionState.Connected, HubConnectionState.Connecting].includes(this.connection.state)) {
    //   if (this.hasError) {
    //     notify('Server connection restored!', 'Success');
    //     this.hasError = false;
    //   }
    // }
    // else if (authService.isAuthenticatied()) {
    //   notify('Server connection problem!', 'Error', this.healthCheckIntervalInMilliseconds);
    //   this.hasError = true;
    // }
  }

  public dispose() {
    this.dropConnection();

    if (this.interval) {
      clearInterval(this.interval);
      this.interval = null;
    }

    // if (this.authSubscription) {
    //   this.authSubscription.unsubscribe();
    //   this.authSubscription = null;
    // }
  }
}

export const hubConnectionService = new HubConnectionService(/* authService,*/ TimeEnum._1second);
