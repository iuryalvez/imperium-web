import React from "react";
import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";
import PaginaEntidadeComCadastro from "../../../components/templates/PaginaSistemaEntidadeComCadastro/index.jsx";
import { Tag } from "antd";

const camposSimplesCadastrarUsuariosDoSistema = [
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
  {
    name: "senha",
    label: "Senha",
    type: "password",
    placeholder: "Digite sua senha",
    rules: [
      { type: "required", message: "Senha obrigatória" },
      { type: "minLength", message: "Mínimo 6 caracteres" },
    ],
  },
];

const camposSimplesEditarUsuariosDoSistema = [
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
    sorter: (a, b) => a.especie.join(", ").localeCompare(b.especie.join(", ")),
    render: (especie) => (
      <>
        {especie.map((perfil) => {
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
      camposSimplesCadastrar={camposSimplesCadastrarUsuariosDoSistema}
      camposSimplesEditar={camposSimplesEditarUsuariosDoSistema}
    />
  );
};

export default UsuariosDoSistema;
