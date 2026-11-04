/**
 * Guarda de rota por permissão da clínica (ver utils/permissoes.js).
 * Sem a permissão, redireciona para o início da clínica (Meu Painel).
 * Props: permissao (ex.: "gerenciaClinica") e children (a página).
 */
import React from "react";
import { Navigate } from "react-router-dom";
import { usePermissao } from "../../utils/usePermissao";

const RotaPorPermissao = ({ permissao, children }) => {
  const pode = usePermissao();
  if (!pode(permissao)) return <Navigate to="/clinica/inicio" replace />;
  return children;
};

export default RotaPorPermissao;
