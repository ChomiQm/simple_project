import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'https://localhost:7197',
  withCredentials: false,
  headers: {
    Accept: 'application/json, text/plain, */*',
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers['Authorization'] = 'Bearer ' + token;
  }
  return config;
}, error => {
  return Promise.reject(error);
});

export default {
  getDocuments(pageIndex, pageSize, search, sortBy, sortDescending) {
    const params = new URLSearchParams();
    params.append('pageIndex', pageIndex);
    params.append('pageSize', pageSize);
    if (search) params.append('search', search);
    if (sortBy) params.append('sortBy', sortBy);
    params.append('sortDescending', sortDescending);

    return apiClient.get(`/documents/getDocuments`, { params });
  },

  postDocument(newDocument) {
  return apiClient.post('/documents/addDocument', newDocument);
  },

   updateDocument(id, updatedDocument) {
    return apiClient.put(`/documents/updateDocument/${id}`, updatedDocument);
  },

  deleteDocument(id){
    return apiClient.delete(`/documents/deleteDocument/${id}`);
  },

  deleteAllDocuments() {
  const params = new URLSearchParams({ confirmation: 'CONFIRM' });
  return apiClient.delete('/documents/deleteAll', { params });
  }
}
