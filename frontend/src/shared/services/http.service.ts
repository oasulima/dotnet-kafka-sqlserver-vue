import notify from 'devextreme/ui/notify';
import createClient, { type Middleware } from 'openapi-fetch';
import type { paths } from '@/lib/api/v1';

function processError(status: number, body: any) {
  if (status == 400) {
    // const errors = body as Record<string, any>;
    // let isDict = true;

    // for (const key of Object.keys(errors)) {
    //   if (!(errors[key] instanceof Array)) {
    //     isDict = false;
    //     break;
    //   }
    // }

    if (body.errors) {
      for (const key of Object.keys(body.errors)) {
        for (const error of body.errors[key]) {
          if (key) {
            notify({ message: `Validation Error! ${key}: ${error}`, type: 'error' }, { position: 'bottom center', direction: 'up-push' });
          } else {
            notify({ message: `Validation Error! ${error}`, type: 'error' }, { position: 'bottom center', direction: 'up-push' });
          }
        }
      }
    }

    // if (!isDict) {
    //   notify(`Validation Error! ${errors['message']}`, 'error');
    // } else {
    //   for (const key of Object.keys(errors)) {
    //     for (const error of errors[key]) {
    //       if (key) {
    //         notify(`Validation Error! ${key}: ${error}`, 'error');
    //       } else {
    //         notify(`Validation Error! ${error}`, 'error');
    //       }
    //     }
    //   }
    // }
  } else {
    notify(`Unexpected Error ${status}`, 'error');
  }
}

const myMiddleware: Middleware = {
  async onResponse({ request, response, options, params }) {
    // console.log('request: ', request);
    // console.log('response: ', response);
    // console.log('options: ', options);
    // console.log('params: ', params);
    const { status, body } = response;
    if (!(options as any).skipErrorProcessing && status >= 400) {
      const data = await response.clone().json();
      processError(status, data);
    }
    // change status of response
    return response;
  },
  onError({ error }) {
    console.log('onError: ', error);
    // notify(`Unexpected Error: ${error}`, 'error');
  }
};

export const client = createClient<paths>({ baseUrl: import.meta.env.VITE_ADMINUI_BASE_URL });

client.use(myMiddleware);
