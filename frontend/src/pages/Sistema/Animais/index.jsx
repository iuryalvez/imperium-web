import React from "react";
import { animaisAPI } from "../../../api/sistema/animaisAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaEntidadeComCadastro/index.jsx";

const camposSimplesCadastrarAnimaisNoSistema = [
  {
    name: "nome",
    label: "Nome do Animal",
    type: "text",
    placeholder: "Digite o nome",
    rules: [{ type: "required", message: "Nome obrigatório" }],
  },
  {
    name: "especie",
    label: "Espécie",
    type: "select",
    placeholder: "Selecione a espécie",
    options: [
      { label: "Cão", value: "1" },
      { label: "Gato", value: "2" },
    ],
    rules: [{ type: "required", message: "Espécie obrigatória" }],
  },
  {
    name: "raca",
    label: "Raça",
    type: "select",
    placeholder: "Selecione a raça",
    options: [],
  },
  {
    name: "nascimento",
    label: "Data de Nascimento",
    type: "date",
    placeholder: "Selecione a data de nascimento",
  },
  {
    name: "sexo",
    label: "Sexo",
    type: "select",
    placeholder: "Selecione o sexo",
    options: [
      { label: "Macho", value: "1" },
      { label: "Fêmea", value: "2" },
    ],
  },
  {
    name: "castrado",
    label: "Castrado",
    type: "boolean",
    // placeholder: "O animal é castrado?",
  },
  {
    name: "peso",
    label: "Peso (kg)",
    type: "number",
    placeholder: "Digite o peso do animal",
  },
  {
    name: "condicaoCorporal",
    label: "Condição Corporal",
    type: "select",
    placeholder: "Selecione a condição corporal",
    options: Array.from({ length: 10 }, (_, i) => ({
      label: (i + 1).toString(),
      value: (i + 1).toString(),
    })),
  },
  {
    name: "grauAtividade",
    label: "Grau de Atividade",
    type: "select",
    placeholder: "Selecione o nível de atividade",
    options: [
      { label: "Inativo", value: "1" },
      { label: "Moderado", value: "2" },
      { label: "Ativo", value: "3" },
    ],
  },
  {
    name: "gestacao",
    label: "Está em gestação?",
    type: "boolean",
    placeholder: "O animal está gestante?",
  },
  {
    name: "lactacao",
    label: "Número de Filhotes",
    type: "number",
    placeholder: "Digite o número de filhotes (se aplicável)",
  },
  {
    name: "semanaLactacao",
    label: "Semana de Lactação",
    type: "number",
    placeholder: "Digite a semana de lactação (se aplicável)",
  },
  {
    name: "semanaGestacao",
    label: "Semana de Gestação",
    type: "number",
    placeholder: "Digite a semana de gestação (se aplicável)",
  },
];

const Animais = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista dos Animais do Sistema"
      entidade="animaisDoSistema"
      api={animaisAPI}
      nomeBotao="Cadastrar Animal"
      nomeEntidade="Animal"
      camposSimplesCadastrar={camposSimplesCadastrarAnimaisNoSistema}
    />
  );
};

export default Animais;
