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
    <div className="flex flex-col">
      <AuthenticatedHeader />
      <div className="flex flex-1 overflow-hidden">
        <Sidebar collapsed={collapsed} toggleCollapse={toggleCollapse} />
        {/* className="flex flex-col mt-8 px-4 md:px-8 lg:px-16 text-left w-full max-w-screen-lg mx-auto space-y-4" */}
        <main className={`flex flex-col mt-12 px-4 md:px-8 lg:px-16 text-left w-full overflow-auto transition-all duration-300 ${collapsed ? 'ml-20' : 'ml-64'}`}>
          <Outlet />
        </main>
      </div>
    </div>
  );
};

export default LayoutAuthenticated;