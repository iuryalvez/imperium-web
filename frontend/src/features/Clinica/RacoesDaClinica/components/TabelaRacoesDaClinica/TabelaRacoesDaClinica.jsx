import React, { useState, useMemo } from "react";
import {
  Table,
  Space,
  Tooltip,
  Button,
  Popconfirm,
  Dropdown,
  Checkbox,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  SettingOutlined,
  CalculatorOutlined,
} from "@ant-design/icons";
import colunas from "./colunas";

const TabelaRacoesDaClinica = ({
  racoes,
  onEdit,
  onDelete,
  onView,
  loading,
}) => {
  const [visibleColumns, setVisibleColumns] = useState(
    colunas.map((c) => c.key),
  );

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key],
    );
  };

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      fixed: "right",
      render: (_, record) => (
        <Space>
          <Tooltip title="Ver Cálculos">
            <Button
              icon={<CalculatorOutlined />}
              size="small"
              onClick={() => onView(record)}
            />
          </Tooltip>
          <Tooltip title="Editar">
            <Button icon={<EditOutlined />} size="small" onClick={() => onEdit(record)} />
          </Tooltip>
          <Tooltip title="Excluir">
            <Popconfirm
              title="Tem certeza que deseja excluir esta ração?"
              onConfirm={() => onDelete(record.id)}
            >
              <Button icon={<DeleteOutlined />} size="small" danger />
            </Popconfirm>
          </Tooltip>
        </Space>
      ),
    }),
    [onEdit, onDelete, onView],
  );

  const colunasVisiveis = useMemo(
    () => [
      ...colunas.filter((col) => visibleColumns.includes(col.key)),
      colunaAcoes,
    ],
    [visibleColumns, colunaAcoes],
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
        dataSource={racoes}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={{ pageSize: 10, showSizeChanger: true }}
        scroll={{ x: "max-content" }}
        loading={loading}
      />
    </div>
  );
};

export default TabelaRacoesDaClinica;
