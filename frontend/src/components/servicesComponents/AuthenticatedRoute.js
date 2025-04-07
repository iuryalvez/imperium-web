import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import Header from '../organisms/Header';

import "../../App.css"
import { Spin } from 'antd';

const AuthenticatedRoute = () => {
  const { isAuthenticated, isLogged, isAdmin, loading } = useAuth();

  if (loading) return (
    <div>
      <Header></Header>
      <div className='main-content'>
        <div className='info-content'>
          <Spin size="large" />;
        </div>
      </div>
    </div>
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
