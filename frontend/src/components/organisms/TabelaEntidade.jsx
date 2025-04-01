import { useState, useEffect } from "react";
import { Table, Button, Space, Tag } from "antd";
import { PlusOutlined, EditOutlined } from "@ant-design/icons";
import "../styles/TabelaEntidade.css";
import { useNavigate } from "react-router-dom";

const TabelaEntidade = ({
    entidade,            // nome da entidade
    dados
  }) => {
  const [registros, setRegistros] = useState(dados);
  
  const navigate = useNavigate();

  useEffect(() => {
    setRegistros(dados);
  }, [dados]);

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
    usuáriosDoSistema: [
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
  };

  const colunasComAcoes = [
    ...(colunasBase[entidade] || []),
    ...(entidade !== "usuários"
      ? [
          {
            title: "Ações",
            key: "acoes",
            render: (_, record) => (
              <Space>
                <Button
                  icon={<EditOutlined />}
                  onClick={() => navigate(`/${entidade}/editar/${record.id}`)}
                  style={{ backgroundColor: "var(--verde-medio)", color: "white" }}
                >
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
          onClick={() => navigate(`/sistema/cadastrar/${entidade}`)}
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
    </div>
  );
};

export default TabelaEntidade;
