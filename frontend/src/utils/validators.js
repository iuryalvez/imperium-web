export const validarCPF = (cpf) => {
  if (!cpf) return false;
  const limpo = cpf.replace(/\D/g, '');
  if (limpo.length !== 11 || !!limpo.match(/(\d)\1{10}/)) return false;

  let tamanho = 9;
  for (let d = 0; d < 2; d++) {
    let soma = 0;
    for (let i = 0; i < tamanho; i++) {
      soma += parseInt(limpo[i]) * ((tamanho + 1) - i);
    }
    let resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(limpo[tamanho])) return false;
    tamanho++;
  }
  return true;
};

export const validarCNPJ = (cnpj) => {
  if (!cnpj) return false;
  const limpo = cnpj.replace(/\D/g, '');
  if (limpo.length !== 14 || !!limpo.match(/(\d)\1{13}/)) return false;

  let tamanho = limpo.length - 2;
  let numeros = limpo.substring(0, tamanho);
  const digitos = limpo.substring(tamanho);
  let soma = 0;
  let pos = tamanho - 7;

  for (let i = tamanho; i >= 1; i--) {
    soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
    if (pos < 2) pos = 9;
  }
  let resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
  if (resultado !== parseInt(digitos.charAt(0))) return false;

  tamanho += 1;
  numeros = limpo.substring(0, tamanho);
  soma = 0;
  pos = tamanho - 7;
  for (let i = tamanho; i >= 1; i--) {
    soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
    if (pos < 2) pos = 9;
  }
  resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
  if (resultado !== parseInt(digitos.charAt(1))) return false;

  return true;
};