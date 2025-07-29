import React from "react";
import { Table, Space, Tooltip, Button, Popconfirm, Tag } from "antd";
import { EditOutlined, DeleteOutlined } from "@ant-design/icons";

const TabelaRacoes = ({ racoes, onEdit, onDelete, loading }) => {
  const colunas = [
    {
      title: "Nome",
      dataIndex: "nome",
      sorter: (a, b) => a.nome.localeCompare(b.nome),
    },
    {
      title: "Espécie",
      dataIndex: "especie",
      render: (especie) => (
        <Tag color={especie === "Cao" ? "blue" : "green"}>{especie}</Tag>
      ),
      filters: [
        { text: "Cão", value: "Cao" },
        { text: "Gato", value: "Gato" },
      ],
      onFilter: (value, record) => record.especie.indexOf(value) === 0,
    },
    {
      title: "Proteína Bruta",
      dataIndex: "proteinaBruta",
      sorter: (a, b) => a.proteinaBruta - b.proteinaBruta,
      render: (value) => `${value.toLocaleString("pt-BR")}%`,
    },
    {
      title: "Matéria Seca",
      dataIndex: "materiaSeca",
      sorter: (a, b) => a.materiaSeca - b.materiaSeca,
      render: (value) => `${value.toLocaleString("pt-BR")}%`,
    },
    {
      title: "Ações",
      key: "acoes",
      render: (_, record) => (
        <Space>
          <Tooltip title="Editar">
            <Button icon={<EditOutlined />} onClick={() => onEdit(record)} />
          </Tooltip>
          <Tooltip title="Excluir">
            <Popconfirm
              title="Tem certeza?"
              onConfirm={() => onDelete(record.id)}
            >
              <Button icon={<DeleteOutlined />} danger />
            </Popconfirm>
          </Tooltip>
        </Space>
      ),
    },
  ];

  return (
    <Table
      dataSource={racoes}
      columns={colunas}
      rowKey="id"
      loading={loading}
    />
  );
};

export default TabelaRacoes;
