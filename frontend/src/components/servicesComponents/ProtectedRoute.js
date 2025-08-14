import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import LoadingSpinner from '../atoms/LoadingSpinner';

const ProtectedRoute = ({
  isPublic = false,      // É uma rota pública?
  requiresLogged = false,  // Requer apenas estar logado?
  requiresAdmin = false,   // Requer ser admin?
  requiresAuth = false,    // Requer ter um vínculo (autenticado)?
}) => {
  const { isLoading, isLogged, isAuthenticated, isAdmin } = useAuth();

  // Show a loading spinner during the initial authentication check.
  // This prevents the "flicker" by not rendering any routes until the check is complete.
  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <LoadingSpinner />
      </div>
    );
  }

  // --- Lógica para Rotas Públicas ---
  if (isPublic) {
    if (isAuthenticated) return <Navigate to="/clinica/inicio" replace />;
    if (isAdmin) return <Navigate to="/sistema" replace />;
    if (isLogged) return <Navigate to="/sistema/vinculos" replace />;
    return <Outlet />; // Se não estiver logado, permite o acesso
  }

  // --- Lógica para Rotas Protegidas ---
  // Se não for pública, a primeira verificação é se o usuário está logado.
  if (!isLogged) {
    return <Navigate to="/login" replace />;
  }

  // Se requer Admin mas o usuário não é
  if (requiresAdmin && !isAdmin) {
    // Redireciona para a página de vínculos se não for admin
    return <Navigate to="/sistema/vinculos" replace />;
  }
  
  // Se requer um vínculo (autenticado) mas o usuário não tem
  if (requiresAuth && !isAuthenticated) {
    // Redireciona para a página apropriada para escolher o vínculo
    return isAdmin ? <Navigate to="/sistema" replace /> : <Navigate to="/sistema/vinculos" replace />;
  }

  // Se a rota requer apenas login, e o usuário está logado mas já autenticado (tem vínculo)
  if (requiresLogged && isAuthenticated) {
    return <Navigate to="/clinica/inicio" replace />;
  }

  // If all checks pass, render the requested component.
  return <Outlet />;
};

export default ProtectedRoute;