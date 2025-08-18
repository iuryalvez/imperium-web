import React, { useState, useMemo } from "react";
import {
  Table,
  Button,
  Space,
  Tooltip,
  Popconfirm,
  Tag,
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
import dayjs from "dayjs";
import { perfisSistema, renderPerfis } from "../../../../utils/renderers";
import { formatarCPF, formatarTelefone } from "../../../../utils/formatters";

const colunas = [
  {
    title: "Nome",
    dataIndex: "nome",
    key: "nome",
    sorter: (a, b) => a.nome.localeCompare(b.nome),
  },
  {
    title: "E-mail",
    dataIndex: "email",
    key: "email",
  },
  {
    title: "Perfis",
    dataIndex: "perfis",
    key: "perfis",
    filters: perfisSistema.map((p) => ({ text: p.label, value: p.value })),
    onFilter: (value, record) => record.perfis.includes(value),
    render: renderPerfis,
  },
  {
    title: "Clínica",
    dataIndex: "nomeClinica",
    key: "nomeClinica",
    // Sorter seguro que lida com valores nulos
    sorter: (a, b) => (a.nomeClinica || "").localeCompare(b.nomeClinica || ""),
    // Renderiza apenas se houver nome de clínica
    render: (text) => text || "N/A",
  },
  {
    title: "Status",
    dataIndex: "isAtivo",
    key: "isAtivo",
    filters: [
      { text: "Ativo", value: true },
      { text: "Inativo", value: false },
    ],
    onFilter: (value, record) => record.isAtivo === value,
    render: (isAtivo) =>
      isAtivo ? <Tag color="green">Ativo</Tag> : <Tag color="red">Inativo</Tag>,
  },
  {
    title: "Data de Cadastro",
    dataIndex: "dataCadastro",
    key: "dataCadastro",
    sorter: (a, b) =>
      dayjs(a.dataCadastro).unix() - dayjs(b.dataCadastro).unix(),
    render: (data) => (data ? dayjs(data).format("DD/MM/YYYY") : "-"),
  },
  {
    title: "Último Acesso",
    dataIndex: "dataUltimoAcesso",
    key: "dataUltimoAcesso",
    sorter: (a, b) =>
      dayjs(a.dataUltimoAcesso).unix() - dayjs(b.dataUltimoAcesso).unix(),
    render: (data) => (data ? dayjs(data).format("DD/MM/YYYY HH:mm") : "Nunca"),
  },
  {
    title: "CPF",
    dataIndex: "cpf",
    key: "cpf",
    render: (cpf) => formatarCPF(cpf),
  },
  {
    title: "Telefone",
    dataIndex: "telefone",
    key: "telefone",
    render: (telefone) => formatarTelefone(telefone),
  },
];

const TabelaUsuariosDoSistema = ({
  data,
  onEdit,
  onToggleStatus,
  onViewDetails,
  loadingRowKey,
}) => {
  // ESTADO INTERNO DO COMPONENTE
  const [visibleColumns, setVisibleColumns] = useState([
    "nome",
    "perfis",
    "nomeClinica",
    "dataCadastro",
  ]);

  const handleColumnVisibilityChange = (key) => {
    setVisibleColumns((prev) =>
      prev.includes(key) ? prev.filter((k) => k !== key) : [...prev, key]
    );
  };

  // 3. DEFINIÇÃO DA COLUNA DE AÇÕES (MEMOIZADA)
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
            <Tooltip title={record.isAtivo ? "Inativar" : "Ativar"}>
              <Popconfirm
                title={`Tem certeza que deseja ${
                  record.isAtivo ? "inativar" : "ativar"
                } este usuário?`}
                onConfirm={() => onToggleStatus(record)}
                okText="Sim"
                cancelText="Não"
                disabled={isCurrentRowLoading}
              >
                <Button
                  size="small"
                  icon={
                    record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />
                  }
                  danger={record.isAtivo}
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

  // 4. CÁLCULO DAS COLUNAS VISÍVEIS (MEMOIZADO)
  // Também usamos useMemo aqui para otimização.
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
        <Dropdown overlay={menuColunas} trigger={["click"]}>
          <Button icon={<SettingOutlined />}>Colunas</Button>
        </Dropdown>
      </div>
      <Table
        dataSource={data}
        columns={colunasVisiveis}
        rowKey={(record) => record.compositeKey || record.id}
        pagination={{ pageSize: 10, showSizeChanger: true }}
        scroll={{ x: "max-content" }}
      />
    </div>
  );
};

export default TabelaUsuariosDoSistema;
