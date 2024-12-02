const API_BASE_URL = 'http://localhost:5050';

const api = {
  post: async (endpoint, body) => {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
      credentials: 'include', // Inclui os cookies na requisição
    });
    return handleResponse(response);
  },
  
  get: async (endpoint) => {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'GET',
      credentials: 'include', // Inclui os cookies na requisição
    });
    return handleResponse(response);
  },

  // Outros métodos HTTP (PUT, DELETE) podem ser adicionados aqui
};

const handleResponse = async (response) => {
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
};

export default api;
