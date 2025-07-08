import React from "react";
import PaginaClinicaEntidadeComCadastro from "../../../components/templates/PaginaClinicaEntidadeComCadastro/index.jsx";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";
import { Tag } from "antd";

const camposSimplesEditarUsuariosDaClinica = [
  {
    name: "nome",
    label: "Nome",
    type: "text",
    placeholder: "Digite seu nome",
    rules: [{ type: "required", message: "Nome obrigatório" }],
  },
  {
    name: "email",
    label: "E-mail",
    type: "email",
    placeholder: "Digite seu e-mail",
    rules: [
      { type: "required", message: "E-mail obrigatório" },
      { type: "email", message: "Formato inválido" },
    ],
  },
];

const colunasUsuariosDaClinica = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: (a, b) => a.nome.localeCompare(b.nome),
  },
  {
    title: "Perfis",
    dataIndex: "perfis",
    key: "perfis",
    filters: [
      { text: "INATIVO", value: "INATIVO" },
      { text: "GESTOR", value: "GESTOR" },
      { text: "TÉCNICO", value: "TECNICO" },
      { text: "SECRETÁRIO", value: "SECRETARIO" },
    ],
    onFilter: (value, record) => record.perfis.includes(value),
    sorter: (a, b) => a.perfis.join(", ").localeCompare(b.perfis.join(", ")),
    render: (perfis) => (
      <>
        {perfis.map((perfil) => {
          let color = "green";
          if (perfil === "INATIVO") color = "red";
          return (
            <Tag color={color} key={perfil}>
              {perfil}
            </Tag>
          );
        })}
      </>
    ),
  },
];

const UsuariosDaClinica = () => {
  return (
    <PaginaClinicaEntidadeComCadastro
      titulo="Lista de Usuários"
      entidade="usuariosDaClinica"
      api={usuariosDaClinicaAPI}
      nomeBotao="Cadastrar Usuário"
      nomeEntidade="Usuário da Clínica"
      colunasBase={colunasUsuariosDaClinica}
      camposSimplesEditar={camposSimplesEditarUsuariosDaClinica}
    />
  );
};

export default UsuariosDaClinica;
