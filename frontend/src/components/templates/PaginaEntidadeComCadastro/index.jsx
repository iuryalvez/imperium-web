import React, { useCallback, useEffect, useState } from "react";
import { Modal, Button } from "antd";
import { ArrowLeftOutlined, PlusOutlined } from "@ant-design/icons";

import LoadingSpinner from "../../atoms/LoadingSpinner.jsx";
import FormCadastrarEntidade from "../../molecules/FormCadastrarEntidade.jsx";
import TabelaEntidade from "../../organisms/TabelaEntidade.jsx";

const camposCadastro = {
  usuariosDoSistema: [
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
  ],
  usuarios: [
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
    {
      name: "perfilId",
      label: "Perfil",
      type: "select",
      placeholder: "Selecione um perfil",
      options: [
        { label: "Gestor", value: 2 },
        { label: "Técnico", value: 5 },
        { label: "Secretário", value: 4 },
        { label: "Veterinário", value: 3 },
      ],
      rules: [{ type: "required", message: "Perfil obrigatório" }],
    },
  ],
  animais: [
      { name: "nome", label: "Nome do Animal", type: "text", placeholder: "Digite o nome", rules: [{ type: "required", message: "Nome obrigatório" }] },
      { name: "especie", label: "Espécie", type: "select", placeholder: "Selecione a espécie", options: [
          { label: "Cão", value: "1" },
          { label: "Gato", value: "2" }
      ], rules: [{ type: "required", message: "Espécie obrigatória" }] },
      { name: "raca", label: "Raça", type: "select", placeholder: "Selecione a raça", options: [] },
      { name: "nascimento", label: "Data de Nascimento", type: "date", placeholder: "Selecione a data de nascimento" },
      { name: "sexo", label: "Sexo", type: "select", placeholder: "Selecione o sexo", options: [
          { label: "Macho", value: "1" },
          { label: "Fêmea", value: "2" }
      ] },
      { name: "castrado", label: "Castrado", type: "boolean", placeholder: "O animal é castrado?" },
      { name: "peso", label: "Peso (kg)", type: "number", placeholder: "Digite o peso do animal" },
      { name: "condicaoCorporal", label: "Condição Corporal", type: "select", placeholder: "Selecione a condição corporal", options: Array.from({ length: 10 }, (_, i) => ({ label: (i + 1).toString(), value: (i + 1).toString() })) },
      { name: "grauAtividade", label: "Grau de Atividade", type: "select", placeholder: "Selecione o nível de atividade", options: [
          { label: "Inativo", value: "1" },
          { label: "Moderado", value: "2" },
          { label: "Ativo", value: "3" }
      ] },
      { name: "gestacao", label: "Está em gestação?", type: "boolean", placeholder: "O animal está gestante?" },
      { name: "lactacao", label: "Número de Filhotes", type: "number", placeholder: "Digite o número de filhotes (se aplicável)" },
      { name: "semanaLactacao", label: "Semana de Lactação", type: "number", placeholder: "Digite a semana de lactação (se aplicável)" },
      { name: "semanaGestacao", label: "Semana de Gestação", type: "number", placeholder: "Digite a semana de gestação (se aplicável)" }
  ],
  tutores: [
      { name: "nome", label: "Nome Completo", type: "text", placeholder: "Digite o nome completo", rules: [{ type: "required", message: "O nome é obrigatório." }, { type: "maxLength", value: 100, message: "O nome deve ter no máximo 100 caracteres." }] },
      { name: "cpf", label: "CPF", type: "text", placeholder: "Digite o CPF (somente números)", rules: [{ type: "required", message: "O CPF é obrigatório." }, { type: "exactLength", value: 11, message: "O CPF deve ter 11 caracteres." }] },
      { name: "email", label: "E-mail", type: "email", placeholder: "Digite o e-mail", rules: [{ type: "required", message: "O e-mail é obrigatório." }, { type: "email", message: "O e-mail não é válido." }] }
  ],
  clinicas: [
      { name: "nomeClinica", label: "Nome da Clínica", type: "text", placeholder: "Digite o nome da clínica", rules: [{ type: "required", message: "O nome da clínica é obrigatório" }, { type: "maxLength", value: 100, message: "O nome deve ter no máximo 100 caracteres" }] }
  ]
};

const PaginaEntidadeComCadastro = ({
  titulo,
  entidade,
  api,
  nomeBotao = "Cadastrar",
}) => {
  const [dados, setDados] = useState([]);
  const [loading, setLoading] = useState(true);
  const [modalVisivel, setModalVisivel] = useState(false);

  const buscarDados = useCallback(async () => {
    try {
      setLoading(true);
      const response = await api.listar();
      setDados(response);
    } catch (err) {
      console.error(`Erro ao buscar ${entidade}: `, err);
    } finally {
      setLoading(false);
    }
  }, [api, entidade]);

  useEffect(() => {
    buscarDados();
  }, [buscarDados]);

  const handleCadastrar = async (formData) => {
    return await api.cadastrar(formData);
  };

  const handleSucesso = () => {
    setModalVisivel(false);
    buscarDados(); // Atualiza a lista
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="main-content">
      <div className="info-content">
        <button
          onClick={() => window.history.back()}
          className="flex items-center font-bold text-green-dark mb-2"
        >
          <ArrowLeftOutlined className="mr-2" /> Voltar
        </button>
        <div className="flex justify-between items-center mb-4">
          <h1 className="text-2xl font-bold text-green-dark">{titulo}</h1>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            className="bg-green-700 hover:bg-green-800 text-white focus:bg-green-800 active:bg-green-900 focus:shadow-none border-none"
            onClick={() => setModalVisivel(true)}
          >
            {nomeBotao}
          </Button>
        </div>

        <TabelaEntidade entidade={entidade} entidadeAPI={api} dados={dados} />
      </div>
      <div className="max-h-[80vh] overflow-y-auto">
        <Modal
          open={modalVisivel}
          onCancel={() => setModalVisivel(false)}
          footer={null}
          centered
          width={600}
          style={{ paddingTop: 24, paddingBottom: 24 }}
        >
          <FormCadastrarEntidade
            entidade={entidade}
            onSubmit={handleCadastrar}
            onSuccess={handleSucesso}
            setModalVisivel={setModalVisivel}
            tituloModal={`Cadastro de ${entidade}`}
            camposCadastro={camposCadastro}
          />
        </Modal>
      </div>
    </div>
  );
};

export default PaginaEntidadeComCadastro;
