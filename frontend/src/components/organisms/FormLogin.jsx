import React, { useState } from 'react';
import InputSemBordaComLabel from '../molecules/InputSemBordaComLabel';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import BotaoPrimario from '../atoms/BotaoPrimario';
import { useAuth } from '../../services/AuthContext';

const FormLogin = () => {
  const { login, isLoading } = useAuth();
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');

  const onSubmit = (e) => {
    e.preventDefault();
    login({ email, senha });
  };

  return (
    <div className="flex flex-col items-center bg-white rounded-3xl shadow-lg py-12 px-12 w-full">
      <h2 className="text-[var(--verde-medio)] text-3xl font-bold mb-4">LOGIN</h2>
      <form className="w-full" onSubmit={onSubmit}>
        <InputSemBordaComLabel
          label="E-mail"
          placeholder="exemplo@email.com"
          icone={<UserOutlined className="text-[var(--verde-medio)]" />}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <InputSemBordaComLabel
          label="Senha"
          type="password"
          placeholder="******"
          icone={<LockOutlined className="text-[var(--verde-medio)]" />}
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
        />

        <div className="flex items-center mt-2 mb-6">
          <input type="checkbox" id="lembrar-me" className="mr-2" />
          <label htmlFor="lembrar-me" className="text-[var(--verde-medio)]">Lembrar-me</label>
        </div>

        <div className="">
          <BotaoPrimario
            texto="Entrar"
            type="submit"
            disabled={isLoading}
          />
        </div>
      </form>

      <div className="text-center mt-4">
        <a href="/recuperar-senha" className="text-[var(--verde-medio)] hover:text-[var(--verde-realce)]">Esqueceu sua senha?</a>
      </div>
    </div>
  );
};

export default FormLogin;
