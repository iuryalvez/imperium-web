import { Card, Typography, Divider, Row, Col } from "antd";
import {
  InfoCircleOutlined,
  ExperimentOutlined,
  CloudServerOutlined,
  HeartOutlined,
  TeamOutlined,
} from "@ant-design/icons";

const { Title, Paragraph } = Typography;

const PaginaSobre = () => {
  return (
    <div style={{ minHeight: "calc(100vh - 4rem)" }}>
      <Row justify="center" style={{ padding: "2rem" }}>
        <Col xs={24} md={20} lg={16}>
          <Card>
            <Title level={2} style={{ textAlign: "center" }}>
              <InfoCircleOutlined style={{ marginRight: 8 }} />
              Sobre o Projeto
            </Title>

            <Paragraph style={{ fontSize: "16px", lineHeight: "1.8" }}>
              Este software tem como objetivo auxiliar no controle nutricional
              de cães e gatos, utilizando o{" "}
              <strong>sistema de regras de Laflamme</strong> para padronizar a
              avaliação do Escore de Condição Corporal (ECC) e as{" "}
              <strong>equações metabólicas do NRC</strong> para o cálculo
              dietético. Com isso, o sistema oferece rigor científico e
              contribui diretamente para a promoção da saúde e do bem-estar dos
              pets.
            </Paragraph>

            <Divider />

            <Row gutter={[16, 16]}>
              <Col xs={24} md={12}>
                <Card>
                  <ExperimentOutlined
                    style={{ fontSize: 24, color: "#722ed1" }}
                  />
                  <Title level={4}>Tecnologia e Ciência</Title>
                  <Paragraph>
                    Utilizando os modelos do NRC (2006), o sistema estima a{" "}
                    <strong>energia metabolizável </strong>
                    de alimentos industrializados a partir das informações
                    bromatológicas dos rótulos, recomendando de forma precisa a
                    quantidade diária ideal de alimento.
                  </Paragraph>
                </Card>
              </Col>

              <Col xs={24} md={12}>
                <Card>
                  <HeartOutlined style={{ fontSize: 24, color: "#eb2f96" }} />
                  <Title level={4}>Foco no Bem-Estar Animal</Title>
                  <Paragraph>
                    As recomendações nutricionais são{" "}
                    <strong>individualizadas</strong> com base em dados como
                    espécie, peso, sexo, idade, nível de atividade e estado
                    fisiológico (como gestação ou lactação).
                  </Paragraph>
                </Card>
              </Col>

              <Col xs={24} md={12}>
                <Card>
                  <CloudServerOutlined
                    style={{ fontSize: 24, color: "#1890ff" }}
                  />
                  <Title level={4}>Arquitetura Moderna</Title>
                  <Paragraph>
                    O sistema está sendo desenvolvido com API própria (backend),
                    interface intuitiva (frontend) e integração via Docker em
                    servidor da LaMCAD/UFG, garantindo escalabilidade e
                    desempenho.
                  </Paragraph>
                </Card>
              </Col>

              <Col xs={24} md={12}>
                <Card>
                  <TeamOutlined style={{ fontSize: 24, color: "#52c41a" }} />
                  <Title level={4}>Equipe Multidisciplinar</Title>
                  <Paragraph>
                    O projeto reúne profissionais de{" "}
                    <strong>Medicina Veterinária</strong>,{" "}
                    <strong>Zootecnia</strong> e
                    <strong> Ciência da Computação</strong>, integrando saberes
                    para um sistema robusto e confiável.
                  </Paragraph>
                </Card>
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default PaginaSobre;
