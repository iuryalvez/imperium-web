import { UserOutlined, MailOutlined, LockOutlined, IdcardOutlined, PhoneOutlined } from '@ant-design/icons';

import BotaoPrimario from '../atoms/BotaoPrimario';
import InputSemBordaComLabel from '../molecules/InputSemBordaComLabel'

// import UserIcon from '../assets/user-icon.svg';
// import MailIcon from '../assets/mail-icon.svg';
// import LockIcon from '../assets/lock-icon.svg';

const FormAutoCadastro = () => {
  return (
    <div className="bg-white rounded-lg shadow-lg pt-12 px-12 w-1/4 max-h-full flex flex-col items-center">
      <h2 className="text-green-dark text-3xl font-bold mb-4">CADASTRO</h2>
      <form className="w-full">
        <InputSemBordaComLabel
          label="Nome"
          placeholder="Como você prefere ser chamado?"
          icone={<UserOutlined className="text-green-dark" />}
        />

        <InputSemBordaComLabel
          label="E-mail"
          type="email"
          placeholder="exemplo@email.com"
          icone={<MailOutlined className="text-green-dark" />}
        />

        <InputSemBordaComLabel
          label="CPF"
          placeholder="XXX.XXX.XXX-XX"
          icone={<IdcardOutlined className="text-green-dark" />}
        />

        <InputSemBordaComLabel
          label="Telefone"
          type="tel"
          placeholder="(XX) XXXXX-XXXX"
          icone={<PhoneOutlined className="text-green-dark" />}
        />

        <InputSemBordaComLabel
          label="Senha"
          type="password"
          placeholder="******"
          icone={<LockOutlined className="text-green-dark" />}
        />

        <InputSemBordaComLabel
          label="Confirme sua senha"
          type="password"
          placeholder="******"
          icone={<LockOutlined className="text-green-dark" />}
        />

        <div className="my-10">
          <BotaoPrimario
            texto="Cadastrar"
            type="submit"
          />
        </div>
      </form>
    </div>
  );
};

export default FormAutoCadastro;
