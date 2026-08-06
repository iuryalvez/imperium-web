import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";

/**
 * Hook customizado para gerenciar toda a lógica da página de Usuários da Clínica.
 */
export const useUsuariosDaClinicaManager = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [loading, setLoading] = useState(true);
  const [loadingRowKey, setLoadingRowKey] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [isOpenModalDetalhes, setIsOpenModalDetalhes] = useState(false);
  const [viewingUser, setViewingUser] = useState(null); // Guarda o usuário a ser visualizado

  const fetchUsuarios = useCallback(async () => {
    setLoading(true);
    try {
      const params = { page, pageSize };
      const response = await usuariosDaClinicaAPI.listar(params);
      setUsuarios(response.data || []);
      setTotal(response.totalCount || 0);
    } catch (error) {
      message.error(error.message);
      setUsuarios([]);
      setTotal(0);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    fetchUsuarios();
  }, [fetchUsuarios]);

  const handleTableChange = (pagination) => {
    setPage(pagination.current);
    setPageSize(pagination.pageSize);
  };

  const handleOpenModalCadastro = () => {
    setEditingUser(null);
    setIsModalOpen(true);
  };

  const handleEditarUsuario = (usuario) => {
    setEditingUser(usuario);
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setEditingUser(null);
  };

  const handleSubmit = async (values) => {
    setIsSubmitting(true);
    let successMessage = "";
    try {
      if (editingUser) {
        // Lógica de Edição
        await usuariosDaClinicaAPI.editar({ ...editingUser, ...values });
        successMessage = "Usuário atualizado com sucesso!";
      } else {
        // Lógica de Cadastro
        await usuariosDaClinicaAPI.cadastrar(values);
        successMessage = "Usuário cadastrado com sucesso!";
      }
      message.success(successMessage);
      handleCloseModal();
      await fetchUsuarios();
    } catch (error) {
      message.error(error.message);
    } finally {
      // O 'finally' garante que o botão de submit pare de carregar, mesmo se a API der erro.
      setIsSubmitting(false);
    }
  };

  const handleToggleStatus = async (usuario) => {
    // Ativa o loading APENAS para esta linha.
    setLoadingRowKey(usuario.id);
    try {
      // Correção: Passando o segundo argumento 'activate'
      const novoStatus = !usuario.isAtivoNaClinica;
      await usuariosDaClinicaAPI.setStatus(usuario, novoStatus);
      const successMessage = `Usuário ${
        novoStatus ? "ativado" : "inativado"
      } com sucesso!`;
      message.success(successMessage);
      await fetchUsuarios(); // Atualiza a tabela com o novo status
    } catch (error) {
      message.error(error.message);
    } finally {
      setLoadingRowKey(null);
    }
  };

  const handleOpenModalDetalhes = (usuario) => {
    setViewingUser(usuario);
    setIsOpenModalDetalhes(true);
  };

  const handleCloseModalDetalhes = () => {
    setIsOpenModalDetalhes(false);
    // Limpa o usuário selecionado após fechar o modal para garantir que ele não "vaze"
    setTimeout(() => setViewingUser(null), 300);
  };

  // Retorna tudo que o componente de UI precisa para funcionar
  return {
    usuarios,
    total,
    page,
    pageSize,
    loading,
    loadingRowKey,
    isModalOpen,
    isOpenModalDetalhes,
    isSubmitting,
    editingUser,
    viewingUser,
    handleTableChange,
    handleOpenModalCadastro,
    handleCloseModal,
    handleCloseModalDetalhes,
    handleSubmit,
    handleEditarUsuario,
    handleToggleStatus,
    handleOpenModalDetalhes,
  };
};
