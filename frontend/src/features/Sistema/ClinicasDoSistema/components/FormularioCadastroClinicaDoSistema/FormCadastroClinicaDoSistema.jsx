import React from "react";
import { Form, Input, Row, Col, Divider, Button, Switch } from "antd";

const FormularioCadastroClinicaDoSistema = ({ onFinish, isSubmitting }) => {
  const [form] = Form.useForm();

  return (
    <Form form={form} layout="vertical" onFinish={onFinish}>
      <Row gutter={24}>
        <Col span={24}>
          <Divider orientation="left" plain>
            Dados Principais
          </Divider>
          
          <Form.Item
            label="Razão Social (Nome)"
            name="nomeClinica"
            rules={[{ required: true, message: "O nome da clínica é obrigatório" }]}
          >
            <Input placeholder="Nome oficial da clínica" maxLength={100} />
          </Form.Item>

          <Form.Item
            label="Status da Clínica"
            name="isAtiva"
            valuePropName="checked"
            initialValue={true} // Define como ativa por padrão
          >
            <Switch checkedChildren="ATIVA" unCheckedChildren="INATIVA" />
          </Form.Item>
        </Col>
      </Row>
      <div className="flex justify-end mt-4">
        <Button type="primary" htmlType="submit" loading={isSubmitting}>
          Cadastrar Clínica
        </Button>
      </div>
    </Form>
  );
};

export default FormularioCadastroClinicaDoSistema;