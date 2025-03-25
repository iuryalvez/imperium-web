import React from 'react';
import { useAuth } from '../../services/AuthContext';
import FormLogin from '../../components/organisms/FormLogin';
import logoUfg from "../../assets/UFG_logo.png";
import logoEvz from "../../assets/EVZ_UFG.svg";
import logoInf from "../../assets/INF_UFG.svg";


const Login = () => {
  const { login } = useAuth();

  const handleLogin = async ({ email, senha }) => {
    login({ email, senha });  // chama a função login do contexto
  };

  return (
    <div className="h-full flex">
      {/* Lado esquerdo - informações */}
      <div className="w-2/3 flex-col items-start text-green-dark pt-20 pr-10 ml-20">
        <h1 className="text-4xl font-bold mb-4">Bem vindo ao Software IMPERIUM!</h1>
        <p className="text-lg mb-8">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer purus arcu, lacinia ultricies nisl et, aliquam iaculis dui. Proin vulputate mollis turpis sed cursus. Curabitur suscipit justo nec vulputate tincidunt. Nulla tincidunt est nibh. Vivamus quis leo non nibh porta imperdiet sed egestas magna. Mauris rhoncus nisi eget purus sodales, ac elementum urna finibus. Etiam dignissim facilisis elit a hendrerit. In eu diam metus. 
        </p>
        <p className="text-green-dark">
          Ainda não possui uma conta? <span className="text-green-dark">Pode se cadastrar </span>
          <a className="font-bold text-green-dark hover:text-green" href="/cadastro">aqui!</a>
        </p>
        <div className="flex gap-4 mt-10">
          <img src={logoUfg} alt="Logo UFG" className="h-16" />
          <img src={logoEvz} alt="Logo EVZ" className="h-16" />
          <img src={logoInf} alt="Logo INF" className="h-16" />
        </div>
      </div>

      {/* Lado direito - formulário */}
      <div className="w-1/2 h-full flex pt-12 items-start justify-center mr-20">
        <FormLogin handleLogin={handleLogin} />
      </div>
    </div>
  );
};

export default Login;
