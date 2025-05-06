import { useState, useEffect } from "react";
import { Table, Button, Space, Tag, Modal, Popconfirm, message } from "antd";
import { DeleteOutlined, EditOutlined, LoginOutlined } from "@ant-design/icons";
import "../styles/TabelaEntidade.css";
import FormEditarEntidade from "../molecules/FormEditarEntidade";
import FormAcessarClinica from "../molecules/FormAcessarClinica";

const camposSimplesEditar = {
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
  usuariosDaClinica: [
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
    {
      name: "nomeClinica",
      label: "Nome da Clínica",
      type: "text",
      placeholder: "Digite o nome oficial da clínica",
      rules: [
        { type: "required", message: "O nome da clínica é obrigatório" },
        { type: "maxLength", value: 100, message: "O nome deve ter no máximo 100 caracteres" }
      ]
    },
    {
      name: "nomeFantasia",
      label: "Nome Fantasia",
      type: "text",
      placeholder: "Digite o nome fantasia da clínica",
      rules: [
        { type: "required", message: "O nome fantasia é obrigatório" },
        { type: "maxLength", value: 100, message: "O nome fantasia deve ter no máximo 100 caracteres" }
      ]
    }
  ]
};

const TabelaEntidade = ({
    entidade,
    nomeEntidade,
    dados,
    entidadeAPI,
    onAtualizar
  }) => {
    const [registros, setRegistros] = useState(dados);
    const [backupRegistros, setBackupRegistros] = useState([]);
    const [modalEdicaoVisivel, setModalEdicaoVisivel] = useState(false);
    const [registroSelecionado, setRegistroSelecionado] = useState(null);
    const [modalAcessoAberto, setModalAcessoAberto] = useState(false);
    const [clinicaSelecionada, setClinicaSelecionada] = useState(null);
    const [loading, setLoading] = useState(false);

    
    useEffect(() => {
      setRegistros(dados);
    }, [dados]);
    
    // Edição otimista com rollback em caso de erro

    const editarRegistro = async (valores) => {
      setLoading(true);
      try {
        // Salva estado atual para possível rollback
        setBackupRegistros(registros);
        
        // Atualização otimista
        setRegistros(prev => 
          prev.map(reg => reg.id === valores.id ? { ...reg, ...valores } : reg)
        );
        
        await entidadeAPI.editar(valores);
        
        setModalEdicaoVisivel(false);
        message.success(`${nomeEntidade} atualizado com sucesso!`);
        
        if (onAtualizar) onAtualizar();
      } catch (erro) {
        // Rollback em caso de erro
        setRegistros(backupRegistros);
        console.error("Erro ao editar registro:", erro);
        message.error(erro.response?.data?.message || 
                    `Falha ao atualizar ${nomeEntidade.toLowerCase()}`);
      } finally {
        setLoading(false);
      }
    };
  
    const deletarRegistro = async (registroId) => {
      setLoading(true);
      try {
        // Salva estado atual
        setBackupRegistros(registros);
        
        // Atualização otimista
        setRegistros(prev => prev.filter(reg => reg.id !== registroId));
        
        await entidadeAPI.deletar(registroId);
        
        message.success(`${nomeEntidade} deletado com sucesso.`);
        
        if (onAtualizar) onAtualizar();
      } catch (error) {
        // Rollback
        setRegistros(backupRegistros);
        console.error("Erro ao deletar registro:", error);
        message.error(error.response?.data?.message || 
                    `Falha ao deletar ${nomeEntidade.toLowerCase()}`);
      } finally {
        setLoading(false);
      }
    };
    
    const abrirModalEdicao = (registro) => {
      setRegistroSelecionado(registro);
      setModalEdicaoVisivel(true);
    };
    
    const abrirModalAcessoClinica = (clinica) => {
      setClinicaSelecionada(clinica);
      setModalAcessoAberto(true);
    };

    const fecharModalAcessoClinica = () => {
      setModalAcessoAberto(false);
      setClinicaSelecionada(null);
    };

  // Configuração das colunas para cada entidade
  const colunasBase = {
    usuariosDaClinica: [
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
        sorter: (a, b) =>
          a.perfis.join(", ").localeCompare(b.perfis.join(", ")),
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
        )
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
        filters: [
          { text: "ADMINISTRADOR", value: "ADMINISTRADOR" },
          { text: "INATIVO", value: "INATIVO" },
          { text: "GESTOR", value: "GESTOR" },
          { text: "TÉCNICO", value: "TECNICO" },
          { text: "SECRETÁRIO", value: "SECRETARIO" },
        ],
        onFilter: (value, record) => record.perfis.includes(value),
        sorter: (a, b) =>
          a.perfis.join(", ").localeCompare(b.perfis.join(", ")),
        render: (perfis) => (
          <>
            {perfis.map((perfil) => {
              let color = "green";
              if (perfil === "INATIVO") color = "red";
              else if (perfil === "ADMINISTRADOR") color = "blue";
              return (
                <Tag color={color} key={perfil}>
                  {perfil}
                </Tag>
              );
            })}
          </>
        )
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
      { 
        title: "Nome da Clínica", 
        dataIndex: "nomeClinica", 
        key: "nomeClinica", 
        sorter: (a, b) => a.nomeClinica.localeCompare(b.nomeClinica) 
      },
      { 
        title: "Status", 
        dataIndex: "isAtiva", 
        key: "isAtiva",
        filters: [
          { text: 'Ativa', value: true },
          { text: 'Inativa', value: false }
        ],
        onFilter: (value, record) => record.isAtiva === value,
        render: (isAtiva) => (
          <Tag color={isAtiva ? "green" : "red"} key={isAtiva ? "ativo" : "inativo"}>
            {isAtiva ? (
              <>
                ATIVA
              </>
            ) : (
              <>
                INATIVA
              </>
            )}
          </Tag>
        ),
        sorter: (a, b) => a.isAtiva - b.isAtiva
      },
      { 
        title: "Data de Cadastro", 
        dataIndex: "dataCad", 
        key: "dataCad",
        render: (dataCad) => new Date(dataCad).toLocaleDateString(),
        sorter: (a, b) => new Date(a.dataCad) - new Date(b.dataCad)
      }
    ]
  };

  const colunasComAcoes = [
    ...(colunasBase[entidade] || []),
    {
      title: "Ações",
      key: "acoes",
      width: "auto",
      render: (_, record) => (
        <Space>
          <Button
            icon={<EditOutlined />}
            onClick={() => abrirModalEdicao(record)}
            style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
          />
          {entidade === "clinicas" && (
            <Button
              icon={<LoginOutlined />}
              onClick={() => abrirModalAcessoClinica(record)}
              style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
            >
              Acessar
            </Button>
          )}
          {entidade !== "usuariosDaClinica" && (
            <Popconfirm
            title="Tem certeza que deseja deletar este registro?"
            onConfirm={() => deletarRegistro(record.id)}
            okText="Sim"
            cancelText="Não"
            >
              <Button
                icon={<DeleteOutlined />}
                style={{ backgroundColor: "var(--vermelho-delete)", color: "white" }}
              />
            </Popconfirm>
          )}
        </Space>
      )
    },
  ];

  return (
    <div>
      <Table
        dataSource={registros}
        columns={colunasComAcoes}
        rowKey={(record) => record.id}
        pagination={{ pageSize: 10 }}
        scroll={{ x: 'max-content' }} // ativa scroll horizontal se necessário
      />
      <div className="max-h-[80vh] overflow-y-auto">
        <Modal
        open={modalEdicaoVisivel}
        onCancel={() => setModalEdicaoVisivel(false)}
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
            setModalEdicaoVisivel={setModalEdicaoVisivel}
            tituloModal={`Edição de ${nomeEntidade}`}
            valoresIniciais={registroSelecionado}
            camposSimplesEditar={camposSimplesEditar}
          />
        </Modal>
        <Modal
          title={`Acessar Clínica`}
          open={modalAcessoAberto}
          onCancel={fecharModalAcessoClinica}
          footer={null}
        >
          <FormAcessarClinica clinica={clinicaSelecionada} onSucesso={fecharModalAcessoClinica} />
        </Modal>
      </div>
    </div>
  );
};

export default TabelaEntidade;
