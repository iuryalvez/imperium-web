import React, { useState } from 'react';
import ReCAPTCHA from "react-google-recaptcha";
import '../App.css'
import '../components/Button.css'
import '../components/Navbar.css'
import logo from '../assets/logo-2.jpeg'

function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [isCaptchaVerified, setIsCaptchaVerified] = useState(false);
  const [showForgotPasswordForm, setShowForgotPasswordForm] = useState(false);
  const [forgotPasswordEmail, setForgotPasswordEmail] = useState('');

  const handleCaptchaChange = (value) => {
    setIsCaptchaVerified(true);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!isCaptchaVerified) {
      alert('Por favor, verifique o Captcha');
      return;
    }
    if (email === 'admin@example.com' && password === 'admin7793!') {
      setIsLoggedIn(true);
    } else {
      alert('Credenciais inválidas');
    }
  };

  const handleForgotPassword = (e) => {
    e.preventDefault();
    // Lógica de redefinição de senha
    alert(`Um e-mail de redefinição de senha foi enviado para ${forgotPasswordEmail}`);
    setForgotPasswordEmail('');
    setShowForgotPasswordForm(false);
  };

  return (
    <div className="App">
      {isLoggedIn ? (
        <div>
          <h2>Bem-vindo, {email}!</h2>
          <button onClick={() => setIsLoggedIn(false)}>Logout</button>
        </div>
      ) : showForgotPasswordForm ? (
        <form onSubmit={handleForgotPassword}>
          <label>
            Insira seu e-mail para recuperar sua senha:
            <br />
            <br />
            <input
              type="email"
              value={forgotPasswordEmail}
              onChange={(e) => setForgotPasswordEmail(e.target.value)}
              required
            />
          </label>
          <br />
          <button type="submit">Enviar</button>
          <button onClick={() => setShowForgotPasswordForm(false)}>Cancelar</button>
        </form>
      ) : (
        <div>
          <form onSubmit={handleSubmit}>
            <img src={logo} alt="Logo" style={{ width: '200px', height: 'auto', padding: '10px' }}/>
            <h1>Login</h1>
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
            <ReCAPTCHA
              sitekey="chave-do-recaptcha-que-nao-tenho"
              onChange={handleCaptchaChange}
            />
            <br />
            <button type="submit">Login</button>
            <button className="forgot-password-button" onClick={() => setShowForgotPasswordForm(true)}>Esqueceu sua senha?</button>
          <br />
          <br />
          <br />
          <br />
          </form>
        </div>
      )}
    </div>
  );
}

export default Login;
