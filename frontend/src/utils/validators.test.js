import { validarCPF, validarCNPJ } from "./validators";

describe("validarCPF", () => {
  test("aceita CPF válido só com dígitos", () => {
    expect(validarCPF("52998224725")).toBe(true);
  });

  test("aceita CPF válido com máscara", () => {
    expect(validarCPF("529.982.247-25")).toBe(true);
  });

  test("recusa dígito verificador errado", () => {
    expect(validarCPF("52998224724")).toBe(false);
  });

  test("recusa todos os dígitos iguais", () => {
    expect(validarCPF("11111111111")).toBe(false);
  });

  test("recusa tamanho diferente de 11", () => {
    expect(validarCPF("5299822472")).toBe(false);
    expect(validarCPF("529982247250")).toBe(false);
  });

  test("recusa vazio, nulo e indefinido", () => {
    expect(validarCPF("")).toBe(false);
    expect(validarCPF(null)).toBe(false);
    expect(validarCPF(undefined)).toBe(false);
  });
});

describe("validarCNPJ", () => {
  test("aceita CNPJ válido com e sem máscara", () => {
    expect(validarCNPJ("11222333000181")).toBe(true);
    expect(validarCNPJ("11.222.333/0001-81")).toBe(true);
  });

  test("recusa dígito verificador errado", () => {
    expect(validarCNPJ("11222333000182")).toBe(false);
  });

  test("recusa todos os dígitos iguais", () => {
    expect(validarCNPJ("00000000000000")).toBe(false);
  });

  test("recusa tamanho diferente de 14 e vazio", () => {
    expect(validarCNPJ("1122233300018")).toBe(false);
    expect(validarCNPJ("")).toBe(false);
    expect(validarCNPJ(null)).toBe(false);
  });
});
