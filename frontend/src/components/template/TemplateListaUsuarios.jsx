import React from 'react';
import { Table, Button, Tag } from 'antd'; // Usando Ant Design para tabela
import Navsidebar from '../organisms/Navsidebar.jsx';

const TemplateListaUsuarios = ({ usuarios }) => {
  const handleDesabilitar = (id) => {
    // Função de desabilitar usuário (pode chamar a API aqui)
    console.log(`Usuário com ID ${id} será desabilitado.`);
  };

  const columns = [
    {
      title: 'Nome',
      dataIndex: 'nome',
      key: 'nome',
    },
    {
      title: 'Habilitado',
      dataIndex: 'habilitado',
      key: 'habilitado',
      render: (habilitado) => (
        <Tag color={habilitado ? 'green' : 'red'}>
          {habilitado ? 'Sim' : 'Não'}
        </Tag>
      ),
    },
    {
      title: 'Perfil',
      dataIndex: 'perfil',
      key: 'perfil',
      sorter: (a, b) => a.cargo.localeCompare(b.cargo), // Ordenação alfabética
    },
    {
      title: 'Data de Cadastro',
      dataIndex: 'dataCadastro',
      key: 'dataCadastro',
      sorter: (a, b) => new Date(a.dataCadastro) - new Date(b.dataCadastro), // Ordenação por data
      render: (data) => new Date(data).toLocaleDateString('pt-BR'), // Formata a data
    },
    {
      title: 'Data Último Acesso',
      dataIndex: 'dataUltimoAcesso',
      key: 'dataUltimoAcesso',
      sorter: (a, b) => new Date(a.dataUltimoAcesso) - new Date(b.dataUltimoAcesso), // Ordenação por data
      render: (data) => new Date(data).toLocaleDateString('pt-BR'), // Formata a data
    },
    {
      title: 'Ações',
      key: 'acoes',
      render: (_, record) => (
        <Button
          type="primary"
          danger
          onClick={() => handleDesabilitar(record.id)}
        >
          Desabilitar
        </Button>
      ),
    },
  ];

  return (
    <div className='main-content'>
      <Navsidebar></Navsidebar>
      <div className='usuarios-content'>
        <h1 className="text-2xl font-bold mb-4 text-green-dark">
            Lista de Usuários
        </h1>
        <Table
          dataSource={usuarios}
          columns={columns}
          rowKey="id" // garante que cada linha tenha um ID único
          pagination={{ pageSize: 10 }} // paginação com 10 registros por página
          />
      </div>
    </div>
  );
};

export default TemplateListaUsuarios;
