import React from "react";
import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaSistemaEntidadeComCadastro/index.jsx";
import { Tag } from "antd";

const colunasUsuariosDoSistema = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: (a, b) => a.nome.localeCompare(b.nome),
  },
  { title: "E-mail", dataIndex: "email", key: "email" },
  {
    title: "Perfis",
    dataIndex: "perfis",
    key: "perfis",
    filters: [
      { text: "ADMIN", value: "ADMIN" },
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
          else if (perfil === "ADMIN") color = "blue";
          return (
            <Tag color={color} key={perfil}>
              {perfil}
            </Tag>
          );
        })}
      </>
    ),
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro),
    render: (data) => new Date(data).toLocaleDateString("pt-BR"),
  },
  {
    title: "Data de Último Acesso",
    dataIndex: "dataUltimoAcesso",
    key: "dataUltimoAcesso",
    sorter: (a, b) =>
      new Date(a.dataUltimoAcesso) - new Date(b.dataUltimoAcesso),
    render: (data) => new Date(data).toLocaleDateString("pt-BR"),
  },
  {
    title: "Nome da Clínica",
    dataIndex: "nomeClinica",
    key: "nomeClinica",
    sorter: (a, b) => a.nomeClinica.localeCompare(b.nomeClinica),
  },
];

const UsuariosDoSistema = () => {
  return (
    <PaginaEntidadeComCadastro
      titulo="Lista de Usuários do Sistema"
      entidade="usuariosDoSistema"
      api={usuariosDoSistemaAPI}
      nomeBotao="Cadastrar Usuário do Sistema"
      nomeEntidade="Usuário do Sistema"
      colunasBase={colunasUsuariosDoSistema}
    />
  );
};

export default UsuariosDoSistema;
