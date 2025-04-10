import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import Header from '../organisms/Header';
import { Spin } from 'antd';

import '../../App.css';

const AdminRoute = () => {
  const { isAdmin, loading, isLogged } = useAuth();

  if (loading) {
    return (
      <div>
        <Header />
        <div className="main-content">
          <div className="info-content">
            <Spin size="large" />
          </div>
        </div>
      </div>
    );
  }

  if (!isLogged) {
    return <Navigate to="/login" />;
  }

  return isAdmin ? <Outlet /> : <Navigate to="/sistema/vinculos" />;
}

export default AdminRoute;
