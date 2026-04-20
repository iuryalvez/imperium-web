/**
 * Remove todos os caracteres não numéricos de um valor.
 * Retorna uma string vazia se o valor for nulo ou indefinido.
 * Útil para limpar máscaras de CPF, CNPJ, CEP e Telefones.
 * * @param {string|number} value - O valor com máscara a ser limpo.
 * @returns {string} - Apenas os números do valor original.
 */
export const sanitizeToNumbers = (value) => {
  if (value === null || value === undefined) return "";
  
  // Converte para string antes de aplicar o replace para evitar erros
  // caso o valor recebido seja um número real e não uma string.
  return String(value).replace(/\D/g, "");
};

/**
 * Varre um objeto e substitui todos os valores null ou undefined por string vazia ("").
 * Essencial para requests PUT/PATCH onde o AutoMapper do backend 
 * ignora propriedades nulas, mas limpa o banco se receber "".
 * * @param {Object} payload - O objeto original vindo do formulário.
 * @returns {Object} - Um novo objeto sanitizado.
 */
export const sanitizePayload = (payload) => {
  if (!payload || typeof payload !== "object") return payload;

  const sanitized = { ...payload };
  Object.keys(sanitized).forEach((key) => {
    if (sanitized[key] === undefined || sanitized[key] === null) {
      sanitized[key] = "";
    }
  });

  return sanitized;
};