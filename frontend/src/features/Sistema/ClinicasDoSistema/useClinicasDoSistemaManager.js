import { useState } from "react";
import { message } from "antd";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI.js";
import { useAuth } from "../../../services/AuthContext.js";
import {
  sanitizePayload,
  sanitizeToNumbers,
} from "../../../utils/sanitizers.js";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosClinica } from "./components/TabelaClinicasDoSistema/colunas";

export const useClinicasDoSistemaManager = () => {
  // Lista paginada no servidor (página, tamanho, busca, ordenação e filtros de coluna)
  const lista = usePaginacaoServidor(clinicasAPI.listar, {}, traduzirFiltrosClinica);
  const clinicas = lista.dados;
  const loading = lista.loading;
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Modais
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isModalDetalhesOpen, setIsModalDetalhesOpen] = useState(false);
  const [isModalAcessoOpen, setIsModalAcessoOpen] = useState(false);

  // Dados selecionados
  const [editingClinica, setEditingClinica] = useState(null);
  const [viewingClinica, setViewingClinica] = useState(null);
  const [accessingClinica, setAccessingClinica] = useState(null);

  const { escolherVinculoAdmin } = useAuth();

  const fetchClinicas = lista.recarregar;

  const handleOpenModalCadastro = () => {
    setEditingClinica(null);
    setIsModalOpen(true);
  };

  const handleEditarClinica = (clinica) => {
    setEditingClinica(clinica);
    setIsModalOpen(true);
  };

  const handleOpenModalDetalhes = (clinica) => {
    setViewingClinica(clinica);
    setIsModalDetalhesOpen(true);
  };

  const handleOpenModalAcesso = (clinica) => {
    setAccessingClinica(clinica);
    setIsModalAcessoOpen(true);
  };

  const handleCloseModals = () => {
    setIsModalOpen(false);
    setIsModalDetalhesOpen(false);
    setIsModalAcessoOpen(false);
    setEditingClinica(null);
    setViewingClinica(null);
    setAccessingClinica(null);
  };

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    try {
      const payload = sanitizePayload({
        ...values,
        cnpj: sanitizeToNumbers(values.cnpj),
        cep: sanitizeToNumbers(values.cep),
        telefone: sanitizeToNumbers(values.telefone),
        whatsapp: sanitizeToNumbers(values.whatsapp),
      });

      if (editingClinica) {
        await clinicasAPI.editar({ ...editingClinica, ...payload });
        message.success("Clínica atualizada com sucesso!");
      } else {
        await clinicasAPI.cadastrar(payload);
        message.success("Clínica cadastrada com sucesso!");
      }

      handleCloseModals();
      await fetchClinicas();
    } catch (error) {
      console.log("Erro ao salvar clínica:", error);
      message.error(
        error.message || "Erro ao salvar clínica.",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSimularAcesso = async (perfis) => {
    try {
      console.log(
        "Simulando acesso para clínica ID:",
        accessingClinica.id,
        "com perfis:",
        perfis,
      );
      await escolherVinculoAdmin(accessingClinica.id, perfis);
      handleCloseModals();
    } catch (error) {
      message.error("Erro ao simular acesso.");
    }
  };

  return {
    paginacao: lista.paginacao,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    clinicas,
    loading,
    isSubmitting,
    isModalOpen,
    isModalDetalhesOpen,
    isModalAcessoOpen,
    editingClinica,
    viewingClinica,
    accessingClinica,
    handleOpenModalCadastro,
    handleEditarClinica,
    handleOpenModalDetalhes,
    handleOpenModalAcesso,
    handleCloseModals,
    handleSubmit,
    handleSimularAcesso,
  };
};
