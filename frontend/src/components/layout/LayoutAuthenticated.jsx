import React, { useState } from 'react';
import AuthenticatedHeader from './Header/AuthenticatedHeader.jsx';
import Sidebar from './Sidebar/index.jsx';
import { Outlet } from 'react-router-dom';

const LayoutAuthenticated = () => {
  const [collapsed, setCollapsed] = useState(false);

  const toggleCollapse = () => {
    setCollapsed(!collapsed);
  };

  return (
    <div className="flex flex-col min-h-screen">
      <AuthenticatedHeader />
      <div className="flex flex-1 overflow-hidden">
        <Sidebar collapsed={collapsed} toggleCollapse={toggleCollapse} />
        <main className={`flex-1 overflow-auto ml-64 transition-all duration-300 ${collapsed ? 'ml-16' : 'ml-min'}`}>
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default LayoutAuthenticated;