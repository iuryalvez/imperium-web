import React from "react";
import { Descriptions, Typography, Divider } from "antd";
import dayjs from "dayjs";

import {
  formatarCEP,
  formatarCPF,
  formatarTelefone,
} from "../../../../../utils/formatters";
import { renderAnimais } from "../../../../../utils/renderers";

const { Text, Link } = Typography;

const FormularioDetalhesTutorDaClinica = ({ valores }) => {
  const naoInformado = <Text type="secondary">Não informado</Text>;

  return (
    <div>
      <Descriptions title="Dados Pessoais" bordered column={{ xs: 1, sm: 2 }}>
        {/* Nome ocupa a linha toda */}
        <Descriptions.Item label="Nome Completo" span={2}>
          {valores.nome ?? naoInformado}
        </Descriptions.Item>

        <Descriptions.Item label="CPF">
          {formatarCPF(valores.cpf) ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Data de Cadastro">
          {valores.dataCadastro
            ? dayjs(valores.dataCadastro).format("DD/MM/YYYY HH:mm")
            : naoInformado}
        </Descriptions.Item>

        {/* Email ocupa a linha toda */}
        <Descriptions.Item label="E-mail" span={2}>
          {valores.email ? (
            <Link href={`mailto:${valores.email}`}>{valores.email}</Link>
          ) : (
            naoInformado
          )}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Contato" bordered column={{ xs: 1, sm: 2 }}>
        <Descriptions.Item label="Telefone Principal">
          {formatarTelefone(valores.telefone) ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="WhatsApp">
          {valores.whatsapp ? (
            <Link
              href={`https://wa.me/55${valores.whatsapp}`}
              target="_blank"
              rel="noopener noreferrer"
            >
              {formatarTelefone(valores.whatsapp)}
            </Link>
          ) : (
            naoInformado
          )}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Endereço" bordered column={1}>
        <Descriptions.Item label="Cidade/Estado">
          {valores.cidade && valores.uf
            ? `${valores.cidade} - ${valores.uf}`
            : valores.cidade || valores.uf || naoInformado}
        </Descriptions.Item>

        <Descriptions.Item label="CEP">
          {formatarCEP(valores.cep) ?? naoInformado}
        </Descriptions.Item>

        <Descriptions.Item label="Bairro">
          {valores.bairro ?? naoInformado}
        </Descriptions.Item>

        <Descriptions.Item label="Logradouro e Número">
          {valores.logradouro
            ? `${valores.logradouro}${valores.endNumero ? `, ${valores.endNumero}` : ""}`
            : naoInformado}
        </Descriptions.Item>

        <Descriptions.Item label="Complemento">
          {valores.complemento ?? naoInformado}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Animais Associados" bordered column={1}>
        <Descriptions.Item label="Animais">
          {valores.animais && valores.animais.length > 0 ? (
            renderAnimais(valores.animais)
          ) : (
            <Text type="secondary">Nenhum animal vinculado no momento.</Text>
          )}
        </Descriptions.Item>
      </Descriptions>
    </div>
  );
};

export default FormularioDetalhesTutorDaClinica;
