import { createApp } from 'vue'
import App from './App.vue'
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap"
import '@fortawesome/fontawesome-free/js/all.js';
import "../src/assets/scss/style.scss"
import axios from "./axios/axios.js"

const app = createApp(App);

app.mount('#app');

app.config.globalProperties.$axios = axios;