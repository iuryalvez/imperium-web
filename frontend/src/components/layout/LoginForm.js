import React, { useState } from 'react';
import ReCAPTCHA from "react-google-recaptcha";
import logo from '../../assets/logo-2.jpeg';
import './LoginForm.css';


const LoginForm = ({ onLogin, onShowForgotPasswordForm }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isCaptchaVerified, setIsCaptchaVerified] = useState(false);

  const handleCaptchaChange = (value) => {
    setIsCaptchaVerified(true);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!isCaptchaVerified) {
      alert('Por favor, verifique o Captcha');
      return;
    }
    onLogin(email, password);
  };

  return (
    <form onSubmit={handleSubmit}>
      <img src={logo} alt="Logo" className='logo-login' />
      <h1>Login</h1>
      <label>
        E-mail
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </label>
      <label>
        Senha
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
      <button type="submit">Login</button>
      <button type="button" className="forgot-password-button" onClick={onShowForgotPasswordForm}>Esqueceu sua senha?</button>
    </form>
  );
};

export default LoginForm;
