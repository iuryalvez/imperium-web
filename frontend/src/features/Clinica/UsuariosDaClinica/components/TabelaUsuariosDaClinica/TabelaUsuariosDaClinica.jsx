import React, { useMemo } from "react";
import { Grid } from "antd";
import {
  EyeOutlined,
  EditOutlined,
  StopOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";
import colunas from './colunas';

// Colunas que aparecem ao abrir a tela; as outras ficam no botão "Colunas".
const COLUNAS_INICIAIS = ["nome", "perfis", "dataCadastro"];

const TabelaUsuariosDaClinica = ({
  usuarios,
  paginacao,
  onChange,
  loading,
  onEdit,
  onToggleStatus,
  onViewDetails,
  loadingRowKey,
  barra, // busca da página, na mesma linha do botão "Colunas"
}) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      width: compacta ? 64 : 120,
      align: "center",
      fixed: "right",
      render: (_, record) => {
        const carregandoLinha = (record.compositeKey || record.id) === loadingRowKey;
        return (
          <AcoesLinha
            compacta={compacta}
            acoes={[
              { chave: "detalhes", rotulo: "Ver detalhes", icone: <EyeOutlined />, onClick: () => onViewDetails(record), desabilitado: carregandoLinha },
              { chave: "editar", rotulo: "Editar", icone: <EditOutlined />, onClick: () => onEdit(record), desabilitado: carregandoLinha },
              {
                chave: "status",
                rotulo: record.isAtivoNaClinica ? "Inativar" : "Ativar",
                icone: record.isAtivoNaClinica ? <StopOutlined /> : <CheckCircleOutlined />,
                perigo: record.isAtivoNaClinica,
                carregando: carregandoLinha,
                confirmar: `Tem certeza que deseja ${record.isAtivoNaClinica ? "inativar" : "ativar"} este usuário?`,
                onClick: () => onToggleStatus(record),
              },
            ]}
          />
        );
      },
    }),
    [onEdit, onToggleStatus, onViewDetails, loadingRowKey, compacta]
  );

  const colunasComAcoes = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <TabelaPadrao
      barra={barra}
      selecionarColunas
      colunasIniciais={COLUNAS_INICIAIS}
      dataSource={usuarios}
      columns={colunasComAcoes}
      rowKey={(record) => record.compositeKey || record.id}
      pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
      onChange={onChange}
      loading={loading}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaUsuariosDaClinica;
