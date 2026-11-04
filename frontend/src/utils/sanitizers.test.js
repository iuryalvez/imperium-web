import { sanitizeToNumbers, sanitizePayload } from "./sanitizers";

describe("sanitizeToNumbers", () => {
  test("tira a máscara de CPF, telefone e CEP", () => {
    expect(sanitizeToNumbers("529.982.247-25")).toBe("52998224725");
    expect(sanitizeToNumbers("(62) 99999-0000")).toBe("62999990000");
    expect(sanitizeToNumbers("74000-000")).toBe("74000000");
  });

  test("aceita número", () => {
    expect(sanitizeToNumbers(62999990000)).toBe("62999990000");
  });

  test("nulo e indefinido viram texto vazio", () => {
    expect(sanitizeToNumbers(null)).toBe("");
    expect(sanitizeToNumbers(undefined)).toBe("");
  });
});

describe("sanitizePayload", () => {
  test("troca nulo e indefinido por texto vazio e mantém o resto", () => {
    const resultado = sanitizePayload({ nome: "Ana", telefone: null, cep: undefined, ativo: false, idade: 0 });

    expect(resultado).toEqual({ nome: "Ana", telefone: "", cep: "", ativo: false, idade: 0 });
  });

  test("não altera o objeto original", () => {
    const original = { telefone: null };

    sanitizePayload(original);

    expect(original.telefone).toBeNull();
  });

  test("devolve como veio o que não é objeto", () => {
    expect(sanitizePayload(null)).toBeNull();
    expect(sanitizePayload("texto")).toBe("texto");
  });
});
