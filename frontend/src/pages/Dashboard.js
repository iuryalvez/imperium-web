import React from 'react';

import '../App.css'
import Navsidebar from '../components/organisms/Navsidebar'

function Dashboard() {
  return (
    <div>
        <Navsidebar></Navsidebar>
        <main className='main-content'>
        </main>
    </div>
  );
}

export default Dashboard;
