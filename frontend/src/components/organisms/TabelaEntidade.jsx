import { useState, useEffect } from "react";
import { Table, Button, Space, Checkbox, Select, message, Modal } from "antd";
import { PlusOutlined, EditOutlined } from "@ant-design/icons";
import { usuariosDaClinicaAPI } from "../../api/clinica/usuariosDaClinicaAPI"; // Exemplo: para usuários. Para outras entidades, ajuste a API conforme necessário.
import CardCadastrarEntidade from "../molecules/CardCadastrarEntidade"; // Componente genérico de cadastro
import "../styles/TabelaEntidade.css";

const TabelaEntidade = ({ 
    entidade,         // Nome da entidade, ex: "usuarios", "animais", etc.
    dados,            // Dados a serem exibidos na tabela
    camposCadastro,   // Configuração dos campos para o card de cadastro (array de objetos)
    onEditar,         // Função de edição, chamada quando se clica em "Editar"
    onSubmitCadastro, // Função que faz a chamada à API para cadastrar
  }) => {
  const [registros, setRegistros] = useState(dados);
  const [loadingId, setLoadingId] = useState(null);
  const [loadingPerfilId, setLoadingPerfilId] = useState(null);
  const [modalVisivel, setModalVisivel] = useState(false);

  useEffect(() => {
    setRegistros(dados);
  }, [dados]);

  const handleToggleHabilitado = async (upcId, checked) => {
    try {
      setLoadingId(upcId);
      if (checked) {
        await usuariosDaClinicaAPI.habilitar(upcId);
        message.success("Usuário habilitado com sucesso!");
      } else {
        await usuariosDaClinicaAPI.desabilitar(upcId);
        message.success("Usuário desabilitado com sucesso!");
      }
      // Atualiza imediatamente o estado local
      setRegistros((prev) =>
        prev.map((item) =>
          item.upcId === upcId ? { ...item, habilitado: checked } : item
        )
      );
    } catch (error) {
      console.error("Erro ao atualizar status:", error);
    } finally {
      setLoadingId(null);
    }
  };

  const handlePerfilChange = async (upcId, novoPerfil) => {
    try {
      setLoadingPerfilId(upcId);
      console.log("Enviando novo perfil:", novoPerfil);
      await usuariosDaClinicaAPI.atualizarPerfil(upcId, novoPerfil);
      message.success("Perfil atualizado com sucesso!");
      setRegistros((prev) =>
        prev.map((item) =>
          item.upcId === upcId ? { ...item, perfil: novoPerfil } : item
        )
      );
    } catch (error) {
      console.error("Erro ao atualizar perfil:", error);
      message.error("Falha ao atualizar perfil.");
    } finally {
      setLoadingPerfilId(null);
    }
  };

  const opcoesPerfil = ["GESTOR", "VETERINARIO", "SECRETARIO", "TECNICO"];

  // Configuração das colunas para cada entidade
  const colunasBase = {
    usuarios: [
      {
        title: "Nome",
        dataIndex: "nome",
        key: "nome",
        sorter: (a, b) => a.nome.localeCompare(b.nome),
      },
      {
        title: "Perfil",
        dataIndex: "perfil",
        key: "perfil",
        render: (perfil, record) => (
          <Select
            value={perfil}
            onChange={(value) => handlePerfilChange(record.upcId, value)}
            loading={loadingPerfilId === record.upcId}
            style={{ width: 150 }}
          >
            {opcoesPerfil.map((opcao) => (
              <Select.Option key={opcao} value={opcao}>
                {opcao}
              </Select.Option>
            ))}
          </Select>
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
        title: "Habilitado", 
        dataIndex: "habilitado", 
        key: "habilitado", 
        render: (habilitado, record) => (
          <Checkbox
            checked={habilitado}
            onChange={(e) => handleToggleHabilitado(record.upcId, e.target.checked)}
            disabled={loadingId === record.upcId}
          />
        ),
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
      { title: "Usuário", dataIndex: "userName", key: "userName" },
      { 
        title: "Perfil", 
        dataIndex: "perfil", 
        key: "perfil",
        render: (perfil, record) => (
          <Select
            value={perfil}
            onChange={(value) => handlePerfilChange(record.upcId, value)}
            loading={loadingPerfilId === record.upcId}
            style={{ width: 150 }}
          >
            {opcoesPerfil.map((opcao) => (
              <Select.Option key={opcao} value={opcao}>
                {opcao}
              </Select.Option>
            ))}
          </Select>
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
      { 
        title: "Habilitado", 
        dataIndex: "habilitado", 
        key: "habilitado",
        render: (habilitado, record) => (
          <Checkbox
            checked={habilitado}
            onChange={(e) => handleToggleHabilitado(record.upcId, e.target.checked)}
            disabled={loadingId === record.upcId}
          />
        ),
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
  };

  const colunasComAcoes = [
    ...(colunasBase[entidade] || []),
    ...(entidade !== "usuarios"
      ? [
          {
            title: "Ações",
            key: "acoes",
            render: (_, record) => (
              <Space>
                <Button
                  icon={<EditOutlined />}
                  onClick={() => onEditar(record)}
                  style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
                >
                  Editar
                </Button>
              </Space>
            ),
          },
        ]
      : []),
  ];

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: 16 }}>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setModalVisivel(true)}
          style={{ backgroundColor: "var(--verde-medio)" }}
        >
          Adicionar {entidade.charAt(0).toUpperCase() + entidade.slice(1)}
        </Button>
      </div>
      <Table
        dataSource={registros}
        columns={colunasComAcoes}
        rowKey={(record) => record.upcId || record.id}
        pagination={{ pageSize: 10 }}
      />
      <Modal
        title={`Cadastrar ${entidade
          .replace(/([A-Z])/g, " $1") // Adiciona um espaço antes de cada letra maiúscula
          .trim() // Remove espaços extras no início, se houver
          .charAt(0)
          .toUpperCase() + entidade.replace(/([A-Z])/g, " $1").trim().slice(1)}`}
        open={modalVisivel}
        onCancel={() => setModalVisivel(false)}
        footer={null}
      >
        <CardCadastrarEntidade
          entidade={entidade}
          campos={camposCadastro}
          onSubmit={onSubmitCadastro}
          onSuccess={(novoRegistro) => {
            setModalVisivel(false);
            setRegistros([...registros, novoRegistro]);
          }}
        />
      </Modal>
    </div>
  );
};

export default TabelaEntidade;
