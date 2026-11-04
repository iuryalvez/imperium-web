import { useCallback } from "react";
import { useAuth } from "../services/AuthContext";
import { temPermissao } from "./permissoes";

/**
 * Hook para checar permissões da clínica pelos perfis do vínculo atual.
 * Uso: const pode = usePermissao(); if (pode("gerenciaAnimais")) { ... }
 */
export const usePermissao = () => {
  const { perfis } = useAuth();
  return useCallback((permissao) => temPermissao(perfis, permissao), [perfis]);
};
