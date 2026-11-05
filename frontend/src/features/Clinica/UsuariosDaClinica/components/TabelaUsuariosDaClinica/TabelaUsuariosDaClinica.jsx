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
  StopOutlined,
  CheckCircleOutlined,
  SettingOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import colunas from './colunas';

const TabelaUsuariosDaClinica = ({
  usuarios,
  paginacao,
  onChange,
  loading,
  onEdit,
  onToggleStatus,
  onViewDetails,
  loadingRowKey,
}) => {
  // ESTADO INTERNO DO COMPONENTE
  const [visibleColumns, setVisibleColumns] = useState([
    "nome",
    "perfis",
    "dataCadastro",
  ]);

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key]
    );
  };

  // DEFINIÇÃO DA COLUNA DE AÇÕES (MEMOIZADA)
  // Usamos useMemo porque esta coluna depende das props (onEdit, etc.).
  // Ela só será recriada se uma dessas funções mudar.
  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: 150,
      align: "center",
      fixed: "right",
      render: (_, record) => {
        const currentRowKey = record.compositeKey || record.id;
        const isCurrentRowLoading = currentRowKey === loadingRowKey;
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
            <Tooltip title={record.isAtivoNaClinica ? "Inativar" : "Ativar"}>
              <Popconfirm
                title={`Tem certeza que deseja ${
                  record.isAtivoNaClinica ? "inativar" : "ativar"
                } este usuário?`}
                onConfirm={() => onToggleStatus(record)}
                okText="Sim"
                cancelText="Não"
                disabled={isCurrentRowLoading}
              >
                <Button
                  size="small"
                  icon={
                    record.isAtivoNaClinica ? <StopOutlined /> : <CheckCircleOutlined />
                  }
                  danger={record.isAtivoNaClinica}
                  loading={isCurrentRowLoading}
                />
              </Popconfirm>
            </Tooltip>
          </Space>
        );
      },
    }),
    [onEdit, onToggleStatus, onViewDetails, loadingRowKey]
  ); // Dependências da coluna de ações

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
        dataSource={usuarios}
        columns={colunasVisiveis}
        rowKey={(record) => record.compositeKey || record.id}
        pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
        onChange={onChange}
        loading={loading}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaUsuariosDaClinica;
