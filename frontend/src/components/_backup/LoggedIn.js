import React from 'react';

const LoggedIn = ({ email, onLogout }) => (
  <div>
    <h2>Bem-vindo, {email}!</h2>
    <button onClick={onLogout}>Logout</button>
  </div>
);

export default LoggedIn;
