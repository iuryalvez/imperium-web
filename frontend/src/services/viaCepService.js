import axios from "axios";

export const viaCepService = {
  buscarCep: async (cep) => {
    const cleanCep = cep.replace(/\D/g, "");
    
    // Early return if CEP is incomplete
    if (cleanCep.length !== 8) return null;

    try {
      const response = await axios.get(`https://viacep.com.br/ws/${cleanCep}/json/`);
      
      // ViaCEP returns a 200 OK with { erro: true } if the CEP doesn't exist
      if (response.data.erro) {
        throw new Error("CEP não encontrado.");
      }
      
      return response.data;
    } catch (error) {
      console.error("[viaCepService] Erro ao buscar CEP:", error);
      throw error;
    }
  }
};