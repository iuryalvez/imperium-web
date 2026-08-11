import React, { useState, useMemo } from "react";
import {
  Table,
  Button,
  Space,
  Popconfirm,
  Tooltip,
  Dropdown,
  Checkbox,
} from "antd";
import {
  LineChartOutlined,
  EditOutlined,
  DeleteOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import colunas from "./colunas";

const TabelaConsultas = ({
  consultas,
  onEdit,
  onDelete,
  onIrParaAcompanhamento,
  loading,
}) => {
  const [visibleColumns, setVisibleColumns] = useState([
    "dataConsulta",
    "nomeAnimal",
    "nomeTutor",
    "nomeUsuario",
    "avaliacaoPreenchida",
  ]);

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key]
    );
  };

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      render: (_, consulta) => (
        <Space size="small">
          <Tooltip title="Editar Agendamento">
            <Button
              size="small"
              icon={<EditOutlined />}
              onClick={() => onEdit(consulta)}
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
              <Button size="small" danger icon={<DeleteOutlined />} />
            </Popconfirm>
          </Tooltip>

          <Tooltip title="Ver Acompanhamento">
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
    }),
    [onEdit, onDelete, onIrParaAcompanhamento]
  );

  const colunasVisiveis = useMemo(
    () => [
      ...colunas.filter((col) => visibleColumns.includes(col.key)),
      colunaAcoes,
    ],
    [visibleColumns, colunaAcoes]
  );

  const menuColunas = (
    <div
      style={{
        padding: 8,
        backgroundColor: "white",
        border: "1px solid #d9d9d9",
        borderRadius: 4,
      }}
    >
      {colunas.map((col) => (
        <div key={col.key}>
          <Checkbox
            checked={visibleColumns.includes(col.key)}
            onChange={() => handleColumnVisibilityChange(col.key)}
          >
            {col.title}
          </Checkbox>
        </div>
      ))}
    </div>
  );

  return (
    <div>
      <div style={{ marginBottom: 16, textAlign: "right" }}>
        <Dropdown dropdownRender={() => menuColunas} trigger={["click"]}>
          <Button icon={<SettingOutlined />}>Colunas</Button>
        </Dropdown>
      </div>
      <Table
        dataSource={consultas}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={{ pageSize: 10, showSizeChanger: true }}
        loading={loading}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaConsultas;
