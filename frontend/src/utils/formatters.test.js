import { render, screen } from "@testing-library/react";
import {
  formatarData,
  formatarDataCadastroPerfil,
  formatarCPF,
  formatarCNPJ,
  formatarTelefone,
  formatarCEP,
  formatarEspecie,
  formatarSexo,
  formatarGrauAtividade,
} from "./formatters";

describe("formatarCPF", () => {
  test("formata 11 dígitos", () => {
    expect(formatarCPF("52998224725")).toBe("529.982.247-25");
  });

  test("devolve o valor original quando não tem 11 dígitos", () => {
    expect(formatarCPF("123")).toBe("123");
    expect(formatarCPF(null)).toBeNull();
  });
});

describe("formatarCNPJ", () => {
  test("formata 14 dígitos", () => {
    expect(formatarCNPJ("11222333000181")).toBe("11.222.333/0001-81");
  });

  test("devolve o valor original quando não tem 14 dígitos", () => {
    expect(formatarCNPJ("1122")).toBe("1122");
  });
});

describe("formatarTelefone", () => {
  test("celular com 11 dígitos", () => {
    expect(formatarTelefone("62999990000")).toBe("(62) 99999-0000");
  });

  test("fixo com 10 dígitos", () => {
    expect(formatarTelefone("6232220000")).toBe("(62) 3222-0000");
  });

  test("devolve o valor original quando tem menos de 10 dígitos", () => {
    expect(formatarTelefone("999")).toBe("999");
    expect(formatarTelefone(undefined)).toBeUndefined();
  });
});

describe("formatarCEP", () => {
  test("formata 8 dígitos", () => {
    expect(formatarCEP("74000000")).toBe("74000-000");
  });

  test("devolve o valor original quando não tem 8 dígitos", () => {
    expect(formatarCEP("7400")).toBe("7400");
  });
});

describe("datas", () => {
  test("formatarData usa DD/MM/YYYY às HH:mm", () => {
    expect(formatarData("2025-03-15T13:45:00")).toBe("15/03/2025 às 13:45");
  });

  test("formatarDataCadastroPerfil usa DD/MM/YYYY", () => {
    expect(formatarDataCadastroPerfil("2025-03-15T13:45:00")).toBe("15/03/2025");
  });

  test("data vazia mostra 'Nunca'", () => {
    render(<div>{formatarData(null)}</div>);
    expect(screen.getByText("Nunca")).toBeInTheDocument();
  });
});

describe("enums", () => {
  test("espécie: 1 é Cão e 2 é Gato", () => {
    expect(formatarEspecie(1)).toBe("Cão");
    expect(formatarEspecie(2)).toBe("Gato");
  });

  test("sexo: 1 é Macho e 2 é Fêmea", () => {
    expect(formatarSexo(1)).toBe("Macho");
    expect(formatarSexo(2)).toBe("Fêmea");
  });

  test("grau de atividade: 1, 2 e 3 têm texto; sem valor mostra aviso", () => {
    expect(formatarGrauAtividade(1)).toBe("Inativo");
    expect(formatarGrauAtividade(2)).toBe("Moderado");
    expect(formatarGrauAtividade(3)).toBe("Alto");
    render(<div>{formatarGrauAtividade(undefined)}</div>);
    expect(screen.getByText("Sem dados de consultas")).toBeInTheDocument();
  });
});
