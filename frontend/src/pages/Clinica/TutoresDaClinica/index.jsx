import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { tutoresDaClinicaAPI } from "../../../api/clinica/tutoresDaClinicaAPI.js";

const camposSimplesCadastrarTutoresDaClinica = [
  {
    name: "nome",
    label: "Nome Completo",
    type: "text",
    placeholder: "Digite o nome completo",
    rules: [
      { type: "required", message: "O nome é obrigatório." },
      {
        type: "maxLength",
        value: 100,
        message: "O nome deve ter no máximo 100 caracteres.",
      },
    ],
  },
  {
    name: "email",
    label: "E-mail",
    type: "email",
    placeholder: "Digite o e-mail",
    rules: [
      { type: "required", message: "O e-mail é obrigatório." },
      { type: "email", message: "O e-mail não é válido." },
    ],
  },
];

const camposSimplesEditarTutoresDaClinica = [
  {
    name: "nome",
    label: "Nome Completo",
    type: "text",
    placeholder: "Digite o nome completo",
    rules: [
      { type: "required", message: "O nome é obrigatório." },
      {
        type: "maxLength",
        value: 100,
        message: "O nome deve ter no máximo 100 caracteres.",
      },
    ],
  },
  {
    name: "email",
    label: "E-mail",
    type: "email",
    placeholder: "Digite o e-mail",
    rules: [
      { type: "required", message: "O e-mail é obrigatório." },
      { type: "email", message: "O e-mail não é válido." },
    ],
  },
];

const colunasTutoresDaClinica = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: (a, b) => a.nome.localeCompare(b.nome),
  },
  { title: "E-mail", dataIndex: "email", key: "email" },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCad",
    key: "dataCadastro",
    sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro),
    render: (data) => new Date(data).toLocaleDateString("pt-BR"),
  },
];

const TutoresDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Tutores"
      entidade="tutoresDaClinica"
      nomeEntidade="Tutor"
      api={tutoresDaClinicaAPI}
      nomeBotao="Cadastrar Tutor"
      colunasBase={colunasTutoresDaClinica}
      camposSimplesCadastrar={camposSimplesCadastrarTutoresDaClinica}
      camposSimplesEditar={camposSimplesEditarTutoresDaClinica}
    />
  );
};

export default TutoresDaClinica;
