import React, { useEffect, useState } from "react";
import { Card, Typography, Spin, Alert, Row, Col } from "antd";
import { autenticacaoAPI } from "../../../api/autenticacaoAPI";
import { useAuth } from "../../../services/AuthContext";

import BotaoSecundario from "../../../components/atoms/BotaoSecundario";

const { Title, Text } = Typography;

const ListaDeVinculos = () => {
  const { escolherVinculo, isLoading } = useAuth();
  const [vinculos, setVinculos] = useState([]);
  const [pageLoading, setPageLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchVinculos = async () => {
      try {
        const response = await autenticacaoAPI.listarVinculos();
        setVinculos(response);
      } catch (err) {
        setError("Erro ao carregar os vínculos.");
      } finally {
        setPageLoading(false);
      }
    };

    fetchVinculos();
  }, []);

  if (pageLoading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <Spin size="large" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto my-8">
        <Title level={2}>Lista de Vínculos</Title>
        <Alert message={error} type="error" showIcon />
      </div>
    );
  }

  return (
<div className="container mx-auto my-8">
      <Title level={2} style={{ color: "var(--verde-escuro)" }}>
        Lista de Vínculos
      </Title>

      <Row gutter={[16, 16]}>
        {vinculos.map((vinculo) => (
          <Col xs={24} md={12} lg={8} key={vinculo.id}>
            <Card
              title={
                <span style={{ color: "var(--verde-escuro)" }}>
                  {vinculo.nomeClinica || "Clínica Desconhecida"}
                </span>
              }
              bordered={false}
              style={{
                boxShadow: "0 2px 8px rgba(0, 0, 0, 0.1)",
                borderRadius: "12px",
              }}
              actions={[
                <BotaoSecundario
                  texto="Selecionar"
                  onClick={() => escolherVinculo(vinculo.id)}
                  disabled={isLoading}
                />,
              ]}
            >
              <Text strong style={{ color: "var(--verde-escuro)" }}>
                Perfis:
              </Text>
              <br />
              <Text style={{ color: "var(--verde-escuro)" }}>
                {vinculo.perfis && vinculo.perfis.length > 0
                  ? vinculo.perfis.join(", ")
                  : "Nenhum perfil vinculado"}
              </Text>
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default ListaDeVinculos;
