import React from "react";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI.js";
import PaginaSistemaEntidadeComCadastro from "../../../components/templates/PaginaSistemaEntidadeComCadastro/index.jsx";
import { Tag } from "antd";

const camposSimplesEditarClinica = [
  {
    name: "nomeClinica",
    label: "Nome da Clínica",
    type: "text",
    placeholder: "Digite o nome oficial da clínica",
    rules: [
      { type: "required", message: "O nome da clínica é obrigatório" },
      {
        type: "maxLength",
        value: 100,
        message: "O nome deve ter no máximo 100 caracteres",
      },
    ],
  },
  {
    name: "nomeFantasia",
    label: "Nome Fantasia",
    type: "text",
    placeholder: "Digite o nome fantasia da clínica",
    rules: [
      {
        type: "maxLength",
        value: 100,
        message: "O nome fantasia deve ter no máximo 100 caracteres",
      },
    ],
  },
];

const colunasClinicas = [
  {
    title: "Nome da Clínica",
    dataIndex: "nomeClinica",
    key: "nomeClinica",
    sorter: (a, b) => a.nomeClinica.localeCompare(b.nomeClinica),
  },
  {
    title: "Status",
    dataIndex: "isAtiva",
    key: "isAtiva",
    filters: [
      { text: "Ativa", value: true },
      { text: "Inativa", value: false },
    ],
    onFilter: (value, record) => record.isAtiva === value,
    render: (isAtiva) => (
      <Tag
        color={isAtiva ? "green" : "red"}
        key={isAtiva ? "ativo" : "inativo"}
      >
        {isAtiva ? <>ATIVA</> : <>INATIVA</>}
      </Tag>
    ),
    sorter: (a, b) => a.isAtiva - b.isAtiva,
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCad",
    key: "dataCad",
    render: (dataCad) => new Date(dataCad).toLocaleDateString(),
    sorter: (a, b) => new Date(a.dataCad) - new Date(b.dataCad),
  },
];

const Clinicas = () => {
  return (
    <PaginaSistemaEntidadeComCadastro
      titulo="Lista de Clínicas do Sistema"
      entidade="clinicas"
      api={clinicasAPI}
      nomeBotao="Cadastrar Clínica"
      nomeEntidade="Clínica"
      colunasBase={colunasClinicas}
      camposSimplesEditar={camposSimplesEditarClinica}
    />
  );
};

export default Clinicas;
