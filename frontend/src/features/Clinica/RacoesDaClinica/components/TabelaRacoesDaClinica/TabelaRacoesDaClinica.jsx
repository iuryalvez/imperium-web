import React, { useMemo } from "react";
import { Grid } from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  CalculatorOutlined,
  StopOutlined,
  CheckCircleOutlined,
  EyeOutlined,
  EyeInvisibleOutlined,
} from "@ant-design/icons";
import colunas, { colunaClinica } from "./colunas";
import TabelaPadrao from "../../../../../components/table/TabelaPadrao";
import AcoesLinha from "../../../../../components/table/AcoesLinha";

// Os nutrientes, a unidade, o status e o cadastro ficam no botão "Colunas".
// "nomeClinica" só existe no modo "copias".
const COLUNAS_INICIAIS = ["nome", "especie", "nomeClinica"];
const COLUNAS_CELULAR = ["nome"];

/**
 * Tabela de rações paginada no servidor. Usada pela clínica e pelo admin (Sistema).
 * modo:
 * - "clinica": ração do sistema pode ser ocultada (não excluída/desativada); ração própria pode ser desativada/excluída.
 * - "sistema": catálogo do sistema (admin).
 * - "copias": rações próprias das clínicas vistas pelo admin (mostra a coluna Clínica).
 * barra: busca e filtros da página, na mesma linha do botão "Colunas".
 */
const TabelaRacoesDaClinica = ({
  racoes,
  loading,
  paginacao,
  onChange,
  modo = "clinica",
  onEdit,
  onDelete,
  onView,
  onToggleStatus,
  onToggleOculta,
  loadingRowKey,
  barra,
  podeEditar = true, // perfis sem "gerenciaRacoes" veem só os cálculos
}) => {
  // No celular as ações viram um menu "⋮"
  const telas = Grid.useBreakpoint();
  const compacta = telas.md === false;

  const colunaAcoes = useMemo(
    () => ({
      title: "Ações",
      key: "acoes",
      fixed: "right",
      align: "center",
      width: compacta ? 64 : undefined,
      render: (_, record) => {
        const carregandoLinha = record.id === loadingRowKey;
        // Na clínica, a ração do sistema só pode ser ocultada; o resto vale para rações próprias e para o admin
        const ehDoSistemaNaClinica = modo === "clinica" && record.isSistema;
        const podeOcultar = podeEditar && ehDoSistemaNaClinica;
        const podeAlterarStatusEExcluir = podeEditar && !ehDoSistemaNaClinica;

        return (
          <AcoesLinha
            compacta={compacta}
            acoes={[
              { chave: "calculos", rotulo: "Ver cálculos", icone: <CalculatorOutlined />, onClick: () => onView(record) },
              podeEditar && {
                chave: "editar",
                rotulo: ehDoSistemaNaClinica ? "Editar (cria uma cópia desta ração para a clínica)" : "Editar",
                icone: <EditOutlined />,
                onClick: () => onEdit(record),
              },
              podeOcultar && {
                chave: "ocultar",
                rotulo: record.isOculta ? "Mostrar para esta clínica" : "Ocultar para esta clínica",
                icone: record.isOculta ? <EyeOutlined /> : <EyeInvisibleOutlined />,
                carregando: carregandoLinha,
                confirmar: record.isOculta ? "Voltar a mostrar esta ração?" : "Ocultar esta ração para a clínica?",
                onClick: () => onToggleOculta(record),
              },
              podeAlterarStatusEExcluir && {
                chave: "status",
                rotulo: record.isAtivo ? "Desativar" : "Ativar",
                icone: record.isAtivo ? <StopOutlined /> : <CheckCircleOutlined />,
                perigo: record.isAtivo,
                carregando: carregandoLinha,
                confirmar: `Tem certeza que deseja ${record.isAtivo ? "desativar" : "ativar"} esta ração?`,
                onClick: () => onToggleStatus(record),
              },
              podeAlterarStatusEExcluir && {
                chave: "excluir",
                rotulo: "Excluir",
                icone: <DeleteOutlined />,
                perigo: true,
                confirmar: "Tem certeza que deseja excluir esta ração?",
                onClick: () => onDelete(record),
              },
            ]}
          />
        );
      },
    }),
    [modo, onEdit, onDelete, onView, onToggleStatus, onToggleOculta, loadingRowKey, podeEditar, compacta],
  );

  const colunasComAcoes = useMemo(
    () => [...(modo === "copias" ? [colunaClinica, ...colunas] : colunas), colunaAcoes],
    [modo, colunaAcoes],
  );

  return (
    <TabelaPadrao
      barra={barra}
      selecionarColunas
      colunasIniciais={COLUNAS_INICIAIS}
      colunasCelular={COLUNAS_CELULAR}
      dataSource={racoes}
      columns={colunasComAcoes}
      rowKey="id"
      pagination={paginacao}
      onChange={onChange}
      scroll={{ x: "max-content" }}
      loading={loading}
    />
  );
};

export default TabelaRacoesDaClinica;
