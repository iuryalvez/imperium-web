import React, { useMemo } from "react";
import { Grid } from "antd";
import { EyeOutlined, EditOutlined, DeleteOutlined, StopOutlined, CheckCircleOutlined } from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";
import colunas from './colunas';

// Colunas que aparecem ao abrir a tela; o endereço fica no botão "Colunas".
// Contato e documentos ficam no botão "Colunas"
const COLUNAS_INICIAIS = ["nome", "isAtivo"];
const COLUNAS_CELULAR = ["nome"];

const TabelaTutoresDaClinica = ({ tutores, paginacao, onChange, loading, onEdit, onDelete, onToggleStatus, onViewDetails, loadingRowKey, barra }) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(() => ({
    title: "Ações",
    key: "acoes",
    width: compacta ? 64 : 150,
    align: "center",
    fixed: "right",
    render: (_, record) => {
      const carregandoLinha = record.id === loadingRowKey;
      return (
        <AcoesLinha
          compacta={compacta}
          acoes={[
            { chave: "detalhes", rotulo: "Ver detalhes", icone: <EyeOutlined />, onClick: () => onViewDetails(record), desabilitado: carregandoLinha },
            { chave: "editar", rotulo: "Editar", icone: <EditOutlined />, onClick: () => onEdit(record), desabilitado: carregandoLinha },
            {
              chave: "status",
              rotulo: record.isAtivo ? "Desativar" : "Ativar",
              icone: record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />,
              perigo: record.isAtivo,
              carregando: carregandoLinha,
              confirmar: `Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} este tutor?`,
              onClick: () => onToggleStatus(record),
            },
            {
              chave: "excluir",
              rotulo: "Remover",
              icone: <DeleteOutlined />,
              perigo: true,
              desabilitado: carregandoLinha,
              confirmar: "Tem certeza que deseja remover este tutor?",
              onClick: () => onDelete(record),
            },
          ]}
        />
      );
    },
  }), [onEdit, onDelete, onToggleStatus, onViewDetails, loadingRowKey, compacta]);

  const colunasComAcoes = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <TabelaPadrao
      barra={barra}
      selecionarColunas
      colunasIniciais={COLUNAS_INICIAIS}
      colunasCelular={COLUNAS_CELULAR}
      dataSource={tutores}
      columns={colunasComAcoes}
      rowKey="id"
      pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
      onChange={onChange}
      loading={loading}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaTutoresDaClinica;
