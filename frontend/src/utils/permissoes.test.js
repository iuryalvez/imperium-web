import { PERMISSOES, PERFIS_DA_CLINICA, temPermissao, somentePerfisDaClinica } from "./permissoes";

describe("PERMISSOES (espelho das policies do backend em Program.cs)", () => {
  test.each([
    ["visualizaTudo", ["GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO"]],
    ["visualizaAtendimento", ["GESTOR", "VETERINARIO", "SECRETARIO"]],
    ["gerenciaClinica", ["GESTOR"]],
    ["gerenciaAnimais", ["GESTOR", "VETERINARIO", "TECNICO"]],
    ["gerenciaRacoes", ["GESTOR", "VETERINARIO", "TECNICO"]],
    ["gerenciaAcompanhamento", ["GESTOR", "VETERINARIO"]],
    ["gerenciaTutores", ["GESTOR", "VETERINARIO", "SECRETARIO"]],
    ["gerenciaConsultas", ["GESTOR", "VETERINARIO", "SECRETARIO"]],
  ])("%s tem os perfis da policy", (permissao, perfis) => {
    expect([...PERMISSOES[permissao]].sort()).toEqual([...perfis].sort());
  });
});

describe("temPermissao", () => {
  test("basta um dos perfis ter a permissão", () => {
    expect(temPermissao(["TECNICO", "SECRETARIO"], "gerenciaTutores")).toBe(true);
  });

  test("TECNICO não acessa atendimento", () => {
    expect(temPermissao(["TECNICO"], "visualizaAtendimento")).toBe(false);
  });

  test("sem perfis ou permissão desconhecida não dá acesso", () => {
    expect(temPermissao(undefined, "visualizaTudo")).toBe(false);
    expect(temPermissao([], "visualizaTudo")).toBe(false);
    expect(temPermissao(["GESTOR"], "naoExiste")).toBe(false);
  });
});

describe("somentePerfisDaClinica", () => {
  test("são os 4 perfis aceitos pelo backend", () => {
    expect(PERFIS_DA_CLINICA).toEqual(["GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO"]);
  });

  test("tira INATIVO e ADMIN e mantém a ordem", () => {
    expect(somentePerfisDaClinica(["VETERINARIO", "INATIVO", "GESTOR", "ADMIN"])).toEqual(["VETERINARIO", "GESTOR"]);
  });

  test("lista vazia ou indefinida vira lista vazia", () => {
    expect(somentePerfisDaClinica(undefined)).toEqual([]);
    expect(somentePerfisDaClinica(["INATIVO"])).toEqual([]);
  });
});
