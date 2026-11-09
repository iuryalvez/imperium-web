import React, { useMemo } from "react";
import { Grid } from "antd";
import { EyeOutlined, EditOutlined, LoginOutlined, DeleteOutlined } from "@ant-design/icons";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";
import colunas from './colunas';

const COLUNAS_CELULAR = ["nomeClinica"];

const TabelaClinicasDoSistema = ({ clinicas, paginacao, onChange, loading, loadingRowKey, onEdit, onViewDetails, onAccess, onDelete, barra }) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(() => ({
    title: "Ações",
    key: "acoes",
    width: compacta ? 64 : 150,
    align: "center",
    fixed: "right",
    render: (_, record) => (
      <AcoesLinha
        compacta={compacta}
        acoes={[
          { chave: "detalhes", rotulo: "Ver detalhes", icone: <EyeOutlined />, onClick: () => onViewDetails(record) },
          { chave: "editar", rotulo: "Editar", icone: <EditOutlined />, onClick: () => onEdit(record) },
          {
            chave: "excluir",
            rotulo: "Excluir",
            icone: <DeleteOutlined />,
            perigo: true,
            carregando: record.id === loadingRowKey,
            confirmar: "Tem certeza que deseja excluir esta clínica?",
            onClick: () => onDelete(record),
          },
          { chave: "acessar", rotulo: "Acessar clínica (simular)", icone: <LoginOutlined />, tipo: "primary", onClick: () => onAccess(record) },
        ]}
      />
    ),
  }), [onEdit, onViewDetails, onAccess, onDelete, loadingRowKey, compacta]);

  const memoColunas = useMemo(() => [...colunas, colunaAcoes], [colunaAcoes]);

  return (
    <TabelaPadrao
      barra={barra}
      selecionarColunas
      colunasCelular={COLUNAS_CELULAR}
      dataSource={clinicas}
      columns={memoColunas}
      rowKey="id"
      pagination={paginacao} // paginação no servidor (usePaginacaoServidor)
      onChange={onChange}
      loading={loading}
      scroll={{ x: "max-content" }}
    />
  );
};

export default TabelaClinicasDoSistema;
