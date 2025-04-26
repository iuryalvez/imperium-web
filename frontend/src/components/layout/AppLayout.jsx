import React from "react";
import Navsidebar from "../organisms/Navsidebar"; // ajuste o path se for diferente

function AppLayout({ children, temSidebar = true }) {
  return (
    <div className="min-h-screen flex">
      {temSidebar && (
        <div>
          <Navsidebar />
        </div>
      )}

      <main
        className={`flex-1 px-6 py-6 transition-all duration-300 ${
          temSidebar ? "ml-0" : "mx-auto max-w-7xl"
        }`}
      >
        {children}
      </main>
    </div>
  );
}

export default AppLayout;