import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { animaisDaClinicaAPI } from "../../../api/clinica/animaisDaClinicaAPI.js";
import { Tag } from "antd";

const camposSimplesCadastrarAnimaisDaClinica = [
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
      { label: "Cão", value: 1 },
      { label: "Gato", value: 2 },
    ],
    rules: [{ type: "required", message: "Espécie obrigatória" }],
  },
  { name: "raca", label: "Raça", type: "select", placeholder: "Selecione a raça", options: [] },
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
      { label: "Macho", value: 1 },
      { label: "Fêmea", value: 2 },
    ],
  },
  // {
  //   name: "isCastrado",
  //   label: "Castrado",
  //   type: "boolean",
  //   // placeholder: "O animal é isCastrado?",
  // },
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
    options: Array.from({ length: 9 }, (_, i) => ({
      label: (i + 1),
      value: (i + 1),
    })),
  },
  {
    name: "grauAtividade",
    label: "Grau de Atividade",
    type: "select",
    placeholder: "Selecione o nível de atividade",
    options: [
      { label: "Inativo", value: 1 },
      { label: "Moderado", value: 2 },
      { label: "Ativo", value: 3 },
    ],
  },
];

const colunasAnimaisDaClinica = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: (a, b) => a.nome.localeCompare(b.nome),
  },
  {
    title: "Espécie",
    dataIndex: "especie",
    key: "especie",
    filters: [
      { text: "Cão", value: 1 },
      { text: "Gato", value: 2 },
    ],
    onFilter: (value, record) => record.especie === value,
    render: (especie) => (
      <Tag
        color={especie === 1 ? "green" : "blue"}
        key={especie === 1 ? "Cao" : "Gato"}
      >
        {especie === 1 ? "Cão" : "Gato"}
      </Tag>
    ),
    sorter: (a, b) => a.especie - b.especie,
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCad",
    key: "dataCadastro",
    sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro),
    render: (data) => new Date(data).toLocaleDateString("pt-BR"),
  },
];

const AnimaisDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Animais"
      entidade="animaisDaClinica"
      api={animaisDaClinicaAPI}
      nomeBotao="Cadastrar Animal"
      nomeEntidade="Animais"
      colunasBase={colunasAnimaisDaClinica}
      camposSimplesCadastrar={camposSimplesCadastrarAnimaisDaClinica}
      camposSimplesEditar={camposSimplesCadastrarAnimaisDaClinica}
    />
  );
};

export default AnimaisDaClinica;
