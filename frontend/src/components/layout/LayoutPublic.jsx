import React from 'react';
import PublicHeader from './Header/PublicHeader.jsx';
import { Outlet } from 'react-router-dom';

const LayoutPublic = () => {
  return (
    <div className="flex flex-col">
      <PublicHeader />
      <main className="px-4 md:px-8 lg:px-16">
        <Outlet />
      </main>
    </div>
  );
};

export default LayoutPublic;