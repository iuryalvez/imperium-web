import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
import { clinicasAPI } from "../../../api/sistema/clinicasAPI.js";
import { useAuth } from "../../../services/AuthContext.js";
import {
  sanitizePayload,
  sanitizeToNumbers,
} from "../../../utils/sanitizers.js";

export const useClinicasDoSistemaManager = () => {
  const [clinicas, setClinicas] = useState([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(true);
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

  const fetchClinicas = useCallback(async () => {
    setLoading(true);
    try {
      const params = { page, pageSize };
      const response = await clinicasAPI.listar(params);
      setClinicas(response.data || []);
      setTotal(response.totalCount || 0);
    } catch (error) {
      message.error(error.message || "Erro ao carregar clínicas.");
      setClinicas([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  // Remover useEffect de reset para searchNome, handleApplyFilters lida com isso

  useEffect(() => {
    fetchClinicas();
  }, [fetchClinicas]);

  const handleTableChange = (pagination) => {
    setPage(pagination.current);
    setPageSize(pagination.pageSize);
  };

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
    clinicas,
    total,
    page,
    pageSize,
    loading,
    isSubmitting,
    isModalOpen,
    isModalDetalhesOpen,
    isModalAcessoOpen,
    editingClinica,
    viewingClinica,
    accessingClinica,
    handleTableChange,
    handleOpenModalCadastro,
    handleEditarClinica,
    handleOpenModalDetalhes,
    handleOpenModalAcesso,
    handleCloseModals,
    handleSubmit,
    handleSimularAcesso,
  };
};
