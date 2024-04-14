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
  getDocumentItemsByDocumentId(documentId) {
    return apiClient.get(`/documentItems/byDocument/${documentId}`);
  },

  postDocumentItemToDocument(documentId, newItem) {
    return apiClient.post(`/documentItems/addItemToDocument/${documentId}`, newItem);
  },

  getAllDocumentItems() {
    return apiClient.get('/documentItems/allItems');
  },

  deleteDocumentItem(documentId, ordinal) {
  return apiClient.delete(`/documentItems/delete${documentId}/item/${ordinal}`);
},

  updateDocumentItem(documentId, ordinal, item) {
    return apiClient.put(`/documentItems/update${documentId}/item/${ordinal}`, item);
},

};
