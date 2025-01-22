import React, { useState } from 'react';
import InputSemBordaComLabel from '../molecules/InputSemBordaComLabel';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import BotaoPrimario from '../atoms/BotaoPrimario';

import logoUfg from "../../assets/UFG_logo.png";
import logoEvz from "../../assets/EVZ_UFG.svg";
import logoInf from "../../assets/INF_UFG.svg";

const FormLogin = ({ handleLogin }) => {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');

  const onSubmit = (e) => {
    e.preventDefault();
    handleLogin({ email, senha });
  };

  return (
    <div className="flex flex-col items-center justify-center bg-white rounded-3xl shadow-lg py-12 px-12 w-1/3">
      <h2 className="text-green-dark text-3xl font-bold mb-4">LOGIN</h2>
      <form className="w-full" onSubmit={onSubmit}>
        <InputSemBordaComLabel
          label="E-mail"
          placeholder="exemplo@email.com"
          icone={<UserOutlined className="text-green-dark" />}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <InputSemBordaComLabel
          label="Senha"
          type="password"
          placeholder="******"
          icone={<LockOutlined className="text-green-dark" />}
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
        />

        <div className="flex items-center mt-2 mb-6">
          <input type="checkbox" id="lembrar-me" className="mr-2" />
          <label htmlFor="lembrar-me" className="text-green-dark">Lembrar-me</label>
        </div>

        <div className="">
          <BotaoPrimario
            texto="Entrar"
            type="submit"
          />
        </div>
      </form>

      <div className="text-center mt-4">
        <a href="/recuperar-senha" className="text-green-dark hover:text-green-light">Esqueceu sua senha?</a>
      </div>
      <div className="flex gap-4 mt-4 pb-10">
          <img src={logoUfg} alt="Logo UFG" className="h-16" />
          <img src={logoEvz} alt="Logo EVZ" className="h-16" />
          <img src={logoInf} alt="Logo INF" className="h-16" />
        </div>
    </div>
  );
};

export default FormLogin;
