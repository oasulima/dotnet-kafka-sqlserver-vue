import { TimeEnum } from '@/constants';
import notify from 'devextreme/ui/notify';

type Listener = { methodName: string, callback: (...args: any[]) => void };

function BuildConnection(listeners: Listener[]) {
  const connection = new EventSource(import.meta.env.VITE_ADMINUI_BASE_URL + 'sse');

  for (const { methodName, callback } of listeners) {
    connection.addEventListener(methodName, (event) => callback(JSON.parse(event.data)));
  }

  return connection;
}

class HubConnectionService {
  private listeners: Listener[] = [];
  private interval: number | null = null;
  private hasError?: boolean;
  private connection: EventSource | null = null;
  private healthCheckIntervalInMilliseconds: number = TimeEnum._1second;


  constructor(
  ) {
    this.interval = setInterval(() => this.handleInterval(), this.healthCheckIntervalInMilliseconds);
    this.connection = BuildConnection(this.listeners);
  }

  public addListner(methodName: string, callback: (...args: any[]) => void) {
    this.listeners.push({ methodName, callback });
    this.connection?.addEventListener(methodName, (event) => {
      return callback(JSON.parse(event.data));
    });
  }

  private handleInterval() {
    if (!this.connection) {
      this.hasError = false;
      return;
    }

    if (this.connection.readyState == EventSource.OPEN) {
      if (this.hasError) {
        notify('Connected', 'Success');
        this.hasError = false;
      }
    }
    else {
      if (this.connection.readyState == EventSource.CLOSED) {
        this.connection.close();
        this.connection = BuildConnection(this.listeners);
      }
      notify('Connecting...', 'Error', this.healthCheckIntervalInMilliseconds);
      this.hasError = true;
    }
  }
}

export const hubConnectionService = new HubConnectionService();
