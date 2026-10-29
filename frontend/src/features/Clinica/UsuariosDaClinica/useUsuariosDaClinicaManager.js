import { useState } from "react";
import { message } from "antd";
import { usuariosDaClinicaAPI } from "../../../api/clinica/usuariosDaClinicaAPI.js";
import { usePaginacaoServidor } from "../../../utils/usePaginacaoServidor";
import { traduzirFiltrosUsuarioClinica } from "./components/TabelaUsuariosDaClinica/colunas";

/**
 * Hook customizado para gerenciar toda a lógica da página de Usuários da Clínica.
 */
export const useUsuariosDaClinicaManager = () => {
  // Lista paginada no servidor (página, tamanho, busca, ordenação e filtros de coluna)
  const lista = usePaginacaoServidor(usuariosDaClinicaAPI.listar, {}, traduzirFiltrosUsuarioClinica);
  const usuarios = lista.dados;
  const loading = lista.loading;
  const [loadingRowKey, setLoadingRowKey] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [isOpenModalDetalhes, setIsOpenModalDetalhes] = useState(false);
  const [viewingUser, setViewingUser] = useState(null); // Guarda o usuário a ser visualizado

  const fetchUsuarios = lista.recarregar;

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
        // Lógica de Edição: a clínica só altera os perfis do vínculo.
        // Dados pessoais e senha são editados pelo próprio usuário (Meu Perfil) ou pelo admin do sistema.
        await usuariosDaClinicaAPI.editar({
          upcId: editingUser.upcId,
          perfis: values.perfis,
        });
        successMessage = "Perfis do usuário atualizados com sucesso!";
      } else {
        // Lógica de Cadastro: se o e-mail já existe no sistema, o backend só vincula o usuário à clínica
        const resultado = await usuariosDaClinicaAPI.cadastrar(values);
        successMessage = resultado?.data?.vinculado
          ? resultado.message
          : "Usuário cadastrado com sucesso!";
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
    paginacao: lista.paginacao,
    setBusca: lista.setBusca,
    handleTableChange: lista.handleTableChange,
    usuarios,
    loading,
    loadingRowKey,
    isModalOpen,
    isOpenModalDetalhes,
    isSubmitting,
    editingUser,
    viewingUser,
    handleOpenModalCadastro,
    handleCloseModal,
    handleCloseModalDetalhes,
    handleSubmit,
    handleEditarUsuario,
    handleToggleStatus,
    handleOpenModalDetalhes,
  };
};
