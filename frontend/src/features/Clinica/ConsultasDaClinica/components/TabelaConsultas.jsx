import React from "react";
import { Table, Button, Tag, Space, Popconfirm, Tooltip } from "antd";
import {
  LineChartOutlined,
  EditOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import dayjs from "dayjs";

const TabelaConsultas = ({
  consultas,
  onEdit,
  onDelete,
  onIrParaAcompanhamento,
  loading,
}) => {
  const colunas = [
    {
      title: "Data da Consulta",
      dataIndex: "dataConsulta",
      render: (data) => dayjs(data).format("DD/MM/YYYY HH:mm"),
      sorter: (a, b) =>
        dayjs(a.dataConsulta).unix() - dayjs(b.dataConsulta).unix(),
    },
    {
      title: "Paciente",
      dataIndex: "nomeAnimal",
      sorter: (a, b) => a.nomeAnimal.localeCompare(b.nomeAnimal),
    },
    {
      title: "Tutor",
      dataIndex: "nomeTutor",
      sorter: (a, b) => a.nomeTutor.localeCompare(b.nomeTutor),
    },
    { title: "Veterinário", dataIndex: "nomeUsuario" },
    {
      title: "Avaliação",
      dataIndex: "avaliacaoPreenchida",
      render: (preenchido) => (
        <Tag color={preenchido ? "green" : "orange"}>
          {preenchido ? "Preenchido" : "Pendente"}
        </Tag>
      ),
      filters: [
        { text: "Preenchido", value: true },
        { text: "Pendente", value: false },
      ],
      onFilter: (value, record) => record.avaliacaoPreenchida === value,
    },
    {
      title: "Ações",
      key: "acoes",
      render: (_, consulta) => (
        <Space size="small">
          <Tooltip title="Editar Agendamento">
            <Button
              type="primary"
              size="small"
              icon={<EditOutlined />}
              onClick={() => onEdit(consulta)}
              ghost
            />
          </Tooltip>

          <Tooltip title="Excluir Consulta">
            <Popconfirm
              title="Excluir consulta"
              description="Tem certeza que deseja excluir esta consulta?"
              onConfirm={() => onDelete(consulta.id)}
              okText="Sim"
              cancelText="Não"
            >
              <Button type="primary" size="small" danger icon={<DeleteOutlined />} ghost />
            </Popconfirm>
          </Tooltip>

          <Tooltip title="Ver Acompanhamento (Ficha/Avaliação/Dieta)">
            <Button
              type="primary"
              size="small"
              icon={<LineChartOutlined />}
              onClick={() => onIrParaAcompanhamento(consulta.animalId)}
              className="bg-verde-medio border-none hover:bg-verde-realce text-white"
            />
          </Tooltip>
        </Space>
      ),
    },
  ];

  return (
    <Table
      dataSource={consultas}
      columns={colunas}
      rowKey="id"
      loading={loading}
    />
  );
};

export default TabelaConsultas;
