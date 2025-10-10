/* eslint-disable no-template-curly-in-string */

export const validateMessages = {
  required: "'${label}' é obrigatório!", // Mensagem para a regra 'required: true'
  types: {
    email: "'${label}' não é um e-mail válido!",
    number: "'${label}' não é um número válido!",
  },
  number: {
    len: "'${label}' deve ter exatamente ${len} dígitos",
    min: "'${label}' deve ser no mínimo ${min}",
    max: "'${label}' deve ser no máximo ${max}",
    range: "'${label}' deve estar entre ${min} e ${max}",
  },
  string: {
    len: "'${label}' deve ter exatamente ${len} caracteres",
    min: "'${label}' deve ter no mínimo ${min} caracteres",
    max: "'${label}' deve ter no máximo ${max} caracteres",
    range: "'${label}' deve ter entre ${min} e ${max} caracteres",
  },
};
