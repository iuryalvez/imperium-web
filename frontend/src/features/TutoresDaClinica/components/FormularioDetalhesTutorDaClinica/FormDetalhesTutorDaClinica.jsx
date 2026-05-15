import React from "react";
import { Descriptions, Typography, Divider, Form } from "antd";
import dayjs from "dayjs";

import {
  formatarCEP,
  formatarCPF,
  formatarTelefone,
} from "../../../../utils/formatters";
import { renderAnimais } from "../../../../utils/renderers";

const FormularioDetalhesTutorDaClinica = ({ valores }) => {
  const naoInformado = (
    <Typography.Text type="secondary">Não informado</Typography.Text>
  );

  return (
    <Form layout="vertical">
      <Descriptions title="Dados Pessoais" bordered column={{ xs: 1, sm: 2 }}>
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
        <Descriptions.Item label="E-mail" span={2}>
          {valores.email ? (
            <Typography.Link href={`mailto:${valores.email}`}>
              {valores.email}
            </Typography.Link>
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
            <Typography.Link
              href={`https://wa.me/55${valores.whatsapp}`}
              target="_blank"
            >
              {formatarTelefone(valores.whatsapp)}
            </Typography.Link>
          ) : (
            naoInformado
          )}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Endereço" bordered column={{ xs: 1, sm: 2 }}>
        <Descriptions.Item label="CEP">
          {formatarCEP(valores.cep) ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Bairro">
          {valores.bairro ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Logradouro" span={2}>
          {valores.logradouro ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Complemento">
          {valores.complemento ?? naoInformado}
        </Descriptions.Item>
        <Descriptions.Item label="Cidade/Estado">
          {valores.cidade && valores.uf 
            ? `${valores.cidade} - ${valores.uf}` 
            : (valores.cidade || valores.uf || naoInformado)}
        </Descriptions.Item>
      </Descriptions>

      <Divider />

      <Descriptions title="Animais Associados" bordered column={1}>
        <Descriptions.Item label="Animais">
          {renderAnimais(valores.animais)}
        </Descriptions.Item>
      </Descriptions>
    </Form>
  );
};

export default FormularioDetalhesTutorDaClinica;