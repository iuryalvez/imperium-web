import React from 'react';
import FormLogin from '../organisms/FormLogin';

const TemplateLogin = ({ handleLogin }) => {
  return (
    <div className="h-full">
      {/* Lado esquerdo - informações */}
      {/* <div className="w-2/3 flex flex-col items-start text-green-dark px-16 pt-20">
        <h1 className="text-4xl font-bold mb-4">Bem vindo ao Boar Reproductive Identifier Software!</h1>
        <p className="text-lg mb-8">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer purus arcu, lacinia ultricies nisl et, aliquam iaculis dui. Proin vulputate mollis turpis sed cursus. Curabitur suscipit justo nec vulputate tincidunt. Nulla tincidunt est nibh. Vivamus quis leo non nibh porta imperdiet sed egestas magna. Mauris rhoncus nisi eget purus sodales, ac elementum urna finibus. Etiam dignissim facilisis elit a hendrerit. In eu diam metus. 
        </p>
        <p className="text-green-dark mb-4">
          Ainda não possui uma conta? <span className="text-green-dark">Pode se cadastrar </span>
          <a className="font-bold text-green-dark hover:text-green" href="/cadastro">aqui!</a>
        </p>
        </div> */}

      {/* Lado direito - formulário */}
      <div className="flex pt-12 items-start justify-center">
        <FormLogin handleLogin={handleLogin} />
      </div>
    </div>
  );
};

export default TemplateLogin;
