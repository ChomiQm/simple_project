import { createApp } from 'vue';
import './style.css';
import App from './App.vue';
import router from './router/router.js';
import store from './stores/AuthStore.js';
import axios from 'axios';

const app = createApp(App);

axios.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

axios.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        const res = await axios.post('https://localhost:7197/refresh', {}, {
          headers: { Authorization: `Bearer ${localStorage.getItem('refreshToken')}` }
        });
        localStorage.setItem('accessToken', res.data.accessToken);
        axios.defaults.headers.common['Authorization'] = `Bearer ${res.data.accessToken}`;
        return axios(originalRequest);
      } catch (refreshError) {
        console.error('Refresh token is invalid', refreshError);
        store.dispatch('logout');
        router.push('/login');
        return Promise.reject(refreshError);
      }
    }
    return Promise.reject(error);
  }
);

app.use(store);
app.use(router);

store.dispatch('tryAutoLogin');

app.mount('#app');
