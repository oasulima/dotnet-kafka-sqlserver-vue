import 'devextreme/dist/css/dx.dark.css';
import './scss/dx-styles.scss';
import './scss/styles.scss';
import { createApp } from 'vue';
import router from './router';

import App from './App.vue';
import { institutionalLocatesService } from './views/institutional-locates/communication.service';

const app = createApp(App);
app.use(router);
app.mount('#app');