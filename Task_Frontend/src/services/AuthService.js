import axios from 'axios';

const apiClientAuth = axios.create({
  baseURL: 'https://localhost:7197',
  withCredentials: false,
  headers: {
    Accept: 'application/json, text/plain, */*',
    'Content-Type': 'application/json'
  }
});

export default {
  register(user) {
    return apiClientAuth.post('/register', user);
  },

  login(user) {
    return apiClientAuth.post('/login', user);
  },

  logout() {
    return apiClientAuth.post('/logout', {}, {
      headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
    });
  }
}
