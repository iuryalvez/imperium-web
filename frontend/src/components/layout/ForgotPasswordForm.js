import React, { useState } from 'react';

const ForgotPasswordForm = ({ onForgotPassword, onCancel }) => {
  const [email, setEmail] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    onForgotPassword(email);
  };

  return (
    <form onSubmit={handleSubmit}>
      <label>
        Insira seu e-mail para recuperar sua senha:
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </label>
      <button type="submit">Enviar</button>
      <button type="button" onClick={onCancel}>Cancelar</button>
    </form>
  );
};

export default ForgotPasswordForm;
