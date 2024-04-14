import axios from 'axios';

// Stwórz instancję axios
const apiClient = axios.create({
  baseURL: 'https://localhost:7197',
  withCredentials: false,
  headers: {
    'Accept': 'application/json, text/plain, */*',
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers['Authorization'] = `Bearer ${token}`;
  }
  return config;
}, error => {
  return Promise.reject(error);
});


export default {
  getActualUserData() {
    return apiClient.get('/userData/getData');
  },

  addUserData(userData) {
    return apiClient.post('/userData/addData', userData);
  },

  hasUserData() {
    return apiClient.post('/userData/hasUserData');
  },

  updateUserData(userDataDto) {
    return apiClient.put(`/userData/updateData`, userDataDto);
  }
};
