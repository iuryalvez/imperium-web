import React, { useState, useMemo } from "react";
import {
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
  LineChartOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import colunas from "./colunas";

const TabelaAnimais = ({
  data,
  onEdit,
  onDelete,
  onViewDetails,
  onNavigateToAcompanhamento,
  loadingRowKey,
}) => {
  // ESTADO INTERNO para controlar as colunas visíveis.
  // Inicializa com todas as colunas visíveis, como pedido.
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
            <Tooltip title="Ver Acompanhamento">
              <Button
                type="primary"
                size="small"
                icon={<LineChartOutlined />}
                onClick={() => onNavigateToAcompanhamento(record.acompanhamentoId)}
                disabled={isCurrentRowLoading || record.acompanhamentoId == null}
              ></Button>
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
      <TabelaPadrao
        dataSource={data}
        columns={colunasVisiveis}
        rowKey="id"
        pagination={{ pageSize: 10, showSizeChanger: true }}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaAnimais;
