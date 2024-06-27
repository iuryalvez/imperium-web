import React, { useState } from 'react'; // Importa o React e a função useState para gerenciar estados.
import '../../App.css'; // Importa o arquivo de estilos globais.
import LoginForm from '../layout/LoginForm'; // Importa o componente de formulário de login.
import ForgotPasswordForm from '../layout/ForgotPasswordForm'; // Importa o componente de formulário de redefinição de senha.
import LoggedIn from '../layout/LoggedIn'; // Importa o componente de estado logado.

function Login() {
  // Define os estados iniciais e suas funções de atualização.
  const [email, setEmail] = useState(''); // Estado para armazenar o e-mail do usuário.
  const [isLoggedIn, setIsLoggedIn] = useState(false); // Estado para verificar se o usuário está logado.
  const [showForgotPasswordForm, setShowForgotPasswordForm] = useState(false); // Estado para mostrar o formulário de redefinição de senha.

  // Função para lidar com o login.
  const handleLogin = (email, password) => {
    // Verifica as credenciais de login.
    if (email === 'admin@example.com' && password === 'admin7793!') {
      setEmail(email); // Define o e-mail no estado.
      setIsLoggedIn(true); // Define o estado de login como verdadeiro.
    } else {
      alert('Credenciais inválidas'); // Exibe uma mensagem de erro.
    }
  };

  // Função para lidar com a redefinição de senha.
  const handleForgotPassword = (email) => {
    alert(`Um e-mail de redefinição de senha foi enviado para ${email}`); // Exibe uma mensagem de sucesso.
    setShowForgotPasswordForm(false); // Oculta o formulário de redefinição de senha.
  };

  // Função para lidar com o logout.
  const handleLogout = () => {
    setIsLoggedIn(false); // Define o estado de login como falso.
    setEmail(''); // Limpa o e-mail no estado.
  };

  return (
    <div>
      {isLoggedIn ? (
        <LoggedIn email={email} onLogout={handleLogout} /> // Renderiza o componente LoggedIn se o usuário estiver logado.
      ) : showForgotPasswordForm ? (
        <ForgotPasswordForm
          onForgotPassword={handleForgotPassword}
          onCancel={() => setShowForgotPasswordForm(false)}
        /> // Renderiza o componente ForgotPasswordForm se o formulário de redefinição de senha estiver visível.
      ) : (
        <LoginForm
          onLogin={handleLogin}
          onShowForgotPasswordForm={() => setShowForgotPasswordForm(true)}
        /> // Renderiza o componente LoginForm se o usuário não estiver logado e o formulário de redefinição de senha não estiver visível.
      )}
    </div>
  );
}

export default Login; // Exporta o componente Login para uso em outras partes do aplicativo.
