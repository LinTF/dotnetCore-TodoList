import { createApp } from 'vue'
import App from './App.vue'
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap"
import '@fortawesome/fontawesome-free/js/all.js';
import "../src/assets/scss/style.scss"
import axios from "./axios/axios.js"
import store from './store'

const app = createApp(App).use(store);

app.mount('#app');

app.config.globalProperties.$axios = axios;