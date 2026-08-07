import { useState, useEffect, useCallback } from "react";
import { message } from "antd";
import { usuariosDoSistemaAPI } from "../../../api/sistema/usuariosDoSistemaAPI.js";

/**
 * Hook customizado para gerenciar toda a lógica da página de Usuários do Sistema.
 */
export const useUsuariosDoSistemaManager = () => {
  const [usuarios, setUsuarios] = useState([]);
  const [loading, setLoading] = useState(true);
  const [loadingRowKey, setLoadingRowKey] = useState(null);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [isDetailsModalVisible, setIsDetailsModalVisible] = useState(false);
  const [viewingUser, setViewingUser] = useState(null); // Guarda o usuário a ser visualizado

  const buscarDados = useCallback(async () => {
    setLoading(true);
    try {
      const dados = await usuariosDoSistemaAPI.listar();
      setUsuarios(dados);
    } catch (error) {
      message.error(error.message);
      setUsuarios([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    buscarDados();
  }, [buscarDados]);

  const handleAbrirModalCadastro = () => {
    setEditingUser(null);
    setIsModalVisible(true);
  };

  const handleEditarUsuario = (usuario) => {
    setEditingUser(usuario);
    setIsModalVisible(true);
  };

  const handleModalCancel = () => {
    setIsModalVisible(false);
    setEditingUser(null);
  };

  const handleFormFinish = async (values) => {
    setIsSubmitting(true);
    let successMessage = "";
    try {
      if (editingUser) {
        // Lógica de Edição
        await usuariosDoSistemaAPI.editar({ ...editingUser, ...values });
        successMessage = "Usuário atualizado com sucesso!";
      } else {
        // Lógica de Cadastro
        await usuariosDoSistemaAPI.cadastrar(values);
        successMessage = "Usuário cadastrado com sucesso!";
      }
      message.success(successMessage);
      handleModalCancel();
      await buscarDados();
    } catch (error) {
      message.error(error.message);
    } finally {
      // O 'finally' garante que o botão de submit pare de carregar, mesmo se a API der erro.
      setIsSubmitting(false);
    }
  };

  const handleToggleStatus = async (usuario) => {
    const rowKey = usuario.compositeKey || usuario.id;
    // Ativa o loading APENAS para esta linha.
    setLoadingRowKey(rowKey);
    try {
      // Correção: Passando o segundo argumento 'activate'
      const novoStatus = !usuario.isAtivo;
      await usuariosDoSistemaAPI.setStatus(usuario, novoStatus);
      const successMessage = `Usuário ${
        novoStatus ? "ativado" : "inativado"
      } com sucesso!`;
      message.success(successMessage);
      await buscarDados(); // Atualiza a tabela com o novo status
    } catch (error) {
      message.error(error.message);
    } finally {
      setLoadingRowKey(null);
    }
  };

  const handleVerDetalhes = (usuario) => {
    setViewingUser(usuario);
    setIsDetailsModalVisible(true);
  };

  const handleDetailsModalCancel = () => {
    setIsDetailsModalVisible(false);
    // Limpa o usuário selecionado após fechar o modal para garantir que ele não "vaze"
    setTimeout(() => setViewingUser(null), 300);
  };

  // Retorna tudo que o componente de UI precisa para funcionar
  return {
    usuarios,
    loading,
    loadingRowKey,
    isModalVisible,
    isDetailsModalVisible,
    isSubmitting,
    editingUser,
    viewingUser,
    handleAbrirModalCadastro,
    handleModalCancel,
    handleDetailsModalCancel,
    handleFormFinish,
    handleEditarUsuario,
    handleToggleStatus,
    handleVerDetalhes,
  };
};
