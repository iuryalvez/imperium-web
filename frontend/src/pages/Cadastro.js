import React, { useState } from 'react';
import '../App.css';

function Cadastro() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isRegistered, setIsRegistered] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    // Lógica de cadastro
    if (password === confirmPassword) {
      setIsRegistered(true);
    } else {
      alert('As senhas não coincidem');
    }
  };

  return (
    <div className="App">
      {isRegistered ? (
        <div>
          <h2>Conta criada com sucesso para {email}!</h2>
        </div>
      ) : (
        <form onSubmit={handleSubmit}>
          <h1>Cadastro</h1>
          <br />
          <label>
            E-mail
            <br />
            <br />
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </label>
          <label>
            Senha
            <br />
            <br />
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </label>
          <label>
            Confirme sua senha
            <br />
            <br />
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          </label>
          <br />
          <button type="submit">Cadastrar</button>
          <br />
          <br />
          <br />
          <br />
        </form>
      )}
    </div>
  );
}

export default Cadastro;
