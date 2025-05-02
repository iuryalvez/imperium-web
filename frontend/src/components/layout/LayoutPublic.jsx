import React from 'react';
import PublicHeader from './Header/PublicHeader.jsx';
import { Outlet } from 'react-router-dom';

const LayoutPublic = () => {
  return (
    <div className="flex flex-col min-h-screen">
      <PublicHeader />
      <main className="flex flex-col mt-8 px-4 md:px-8 lg:px-16 text-left w-full min-w-screen-lg mx-auto space-y-4">
        <Outlet />
      </main>
    </div>
  );
};

export default LayoutPublic;