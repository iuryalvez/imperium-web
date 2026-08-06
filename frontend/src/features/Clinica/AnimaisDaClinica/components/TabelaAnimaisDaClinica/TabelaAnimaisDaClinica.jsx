import React, { useState, useMemo } from "react";
import {
  Table,
  Button,
  Space,
  Tooltip,
  Popconfirm,
  Dropdown,
  Checkbox,
} from "antd";
import {
  EyeOutlined,
  EditOutlined,
  DeleteOutlined,
  SettingOutlined,
  SolutionOutlined,
} from "@ant-design/icons";
import colunas from "./colunas";

const TabelaAnimais = ({
  data,
  total,
  page,
  pageSize,
  onTableChange,
  onEdit,
  onDelete,
  onViewDetails,
  onNavigateToAcompanhamento,
  loadingRowKey,
}) => {
  const [visibleColumns, setVisibleColumns] = useState([
    "nome",
    "especie",
    "racaNome",
    "dataCadastro",
  ]);

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key]
    );
  };

  // DEFINIÇÃO DA COLUNA DE AÇÕES (MEMOIZADA)
  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: 150,
      align: "center",
      fixed: "right",
      render: (_, record) => {
        const isCurrentRowLoading = record.id === loadingRowKey; // Supondo loading por ID
        return (
          <Space>
            <Tooltip title="Ver Detalhes">
              <Button
                size="small"
                icon={<EyeOutlined />}
                onClick={() => onViewDetails(record)}
                disabled={isCurrentRowLoading}
              />
            </Tooltip>
            <Tooltip title="Editar">
              <Button
                size="small"
                icon={<EditOutlined />}
                onClick={() => onEdit(record)}
                disabled={isCurrentRowLoading}
              />
            </Tooltip>
            <Tooltip title="Acompanhamento">
              <Button
                size="small"
                icon={<SolutionOutlined />}
                onClick={() => onNavigateToAcompanhamento(record.acompanhamentoId)}
                disabled={isCurrentRowLoading || record.acompanhamentoId == null}
              />
            </Tooltip>
            <Tooltip title="Excluir">
              <Popconfirm
                title="Tem certeza que deseja excluir?"
                onConfirm={() => onDelete(record.id)}
              >
                <Button
                  size="small"
                  icon={<DeleteOutlined />}
                  danger
                  disabled={isCurrentRowLoading}
                />
              </Popconfirm>
            </Tooltip>
          </Space>
        );
      },
    }),
    [onEdit, onDelete, onNavigateToAcompanhamento, onViewDetails, loadingRowKey]
  );

  // CÁLCULO DAS COLUNAS VISÍVEIS (MEMOIZADO)
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
        dataSource={data}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={{
          current: page,
          pageSize: pageSize,
          total: total,
          showSizeChanger: true,
        }}
        onChange={onTableChange}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaAnimais;
