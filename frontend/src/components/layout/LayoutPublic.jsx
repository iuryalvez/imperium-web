import React from 'react';
import PublicHeader from './Header/PublicHeader.jsx';
import { Outlet } from 'react-router-dom';

const LayoutPublic = () => {
  return (
    <div className="flex flex-col min-h-screen">
      <PublicHeader />
      <main>
        <Outlet />
      </main>
    </div>
  );
};

export default LayoutPublic;