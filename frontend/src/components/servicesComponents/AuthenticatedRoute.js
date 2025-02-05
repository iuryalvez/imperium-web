import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';

const AuthenticatedRoute = () => {
  const { isAuthenticated, isLogged, isAdmin, loading } = useAuth();

  if (loading) return (
    <div>Carregando...</div> 
  );

  if (isAuthenticated) {
    return <Outlet></Outlet>
  }
  if (isLogged) {
    return isAdmin ? <Navigate to="/sistema" /> 
    : <Navigate to="/sistema/vinculos" />
  }

  return <Navigate to="/login" />;

};

export default AuthenticatedRoute;
