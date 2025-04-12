import { useState, useEffect } from "react";
import { Table, Button, Space, Tag, Modal } from "antd";
import { EditOutlined } from "@ant-design/icons";
import "../styles/TabelaEntidade.css";
import FormEditarEntidade from "../molecules/FormEditarEntidade";

const camposEditar = {
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
    }
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

const TabelaEntidade = ({
    entidade,            // nome da entidade
    dados,
    entidadeAPI,
    onAtualizar
  }) => {
    const [registros, setRegistros] = useState(dados);
    const [modalVisivel, setModalVisivel] = useState(false);
    const [registroSelecionado, setRegistroSelecionado] = useState(null);
  
    useEffect(() => {
      setRegistros(dados);
    }, [dados]);

    // Envia os dados editados para a API
    const editarRegistro = async (valores) => {
      try {
        console.log("Valores iniciais passados para edição:", dados);
        console.log("Valores recebidos para edição:", valores);
        await entidadeAPI.editar(valores); // função do seu API service
        setModalVisivel(false);
      } catch (erro) {
        console.error("Erro ao editar registro:", erro);
      }
    };  

  const abrirModalEdicao = (registro) => {
    setRegistroSelecionado(registro);
    setModalVisivel(true);
  };

  // Configuração das colunas para cada entidade
  const colunasBase = {
    usuáriosDaClínica: [
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
        render: (perfis) => (
          <>
            {perfis.map((perfil) => (
              <Tag color="blue" key={perfil}>
                {perfil}
              </Tag>
            ))}
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
        sorter: (a, b) => new Date(a.dataUltimoAcesso) - new Date(b.dataUltimoAcesso),
        render: (data) => new Date(data).toLocaleDateString("pt-BR"),
      },
    ],
    usuariosDoSistema: [
      { 
        title: "Nome", 
        dataIndex: "nome", 
        key: "nome", 
        sorter: (a, b) => a.nome.localeCompare(b.nome) 
      },
      { title: "E-mail", dataIndex: "email", key: "email" },
      {
        title: "Perfis",
        dataIndex: "perfis",
        key: "perfis",
        render: (perfis) => (
          <>
            {perfis.map((perfil) => (
              <Tag color="green" key={perfil}>
                {perfil}
              </Tag>
            ))}
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
        sorter: (a, b) => new Date(a.dataUltimoAcesso) - new Date(b.dataUltimoAcesso),
        render: (data) => new Date(data).toLocaleDateString("pt-BR"),
      },
      { 
        title: "Nome da Clínica", 
        dataIndex: "nomeClinica", 
        key: "nomeClinica",
        sorter: (a, b) => a.nomeClinica.localeCompare(b.nomeClinica),
      },
    ],        
    animais: [
      { title: "Nome", dataIndex: "nome", key: "nome", sorter: (a, b) => a.nome.localeCompare(b.nome) },
      { title: "Espécie", dataIndex: "especie", key: "especie" },
      { title: "Idade", dataIndex: "idade", key: "idade" },
    ],
    tutores: [
      { title: "Nome", dataIndex: "nome", key: "nome", sorter: (a, b) => a.nome.localeCompare(b.nome) },
      { title: "E-mail", dataIndex: "email", key: "email" },
    ],
    clinicas: [
      { title: "Nome da Clínica", dataIndex: "nomeClinica", key: "nomeClinica", sorter: (a, b) => a.nomeClinica.localeCompare(b.nomeClinica) }
    ],
  };

  const colunasComAcoes = [
    ...(colunasBase[entidade] || []),
    ...[
      {
        title: "Ações",
          key: "acoes",
          render: (_, record) => (
            <Space>
              <Button
                icon={<EditOutlined />}
                onClick={() => abrirModalEdicao(record)}
                style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
              />
            </Space>
        ),
      },
    ],
  ];

  return (
    <div>
      <Table
        dataSource={registros}
        columns={colunasComAcoes}
        rowKey={(record) => record.upcId || record.id}
        pagination={{ pageSize: 10 }}
      />
      <div className="max-h-[80vh] overflow-y-auto">
        <Modal
        open={modalVisivel}
        onCancel={() => setModalVisivel(false)}
        footer={null}
        centered
        destroyOnClose
        style={{ paddingTop: 24, paddingBottom: 24 }}
        className="modal-edicao"
        >
          <FormEditarEntidade
            entidade={entidade}
            onSubmit={(valores) => editarRegistro(valores)}
            onSuccess={onAtualizar}
            setModalVisivel={setModalVisivel}
            tituloModal={`Edição de ${entidade}`}
            valoresIniciais={registroSelecionado}
            camposEditar={camposEditar}
          />
        </Modal>
      </div>
    </div>
  );
};

export default TabelaEntidade;
