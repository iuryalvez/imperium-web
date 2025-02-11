import axios from 'axios';

const api = axios.create({
  baseURL: 'https://' + window.location.hostname,
  withCredentials: true,
});

export default api;
