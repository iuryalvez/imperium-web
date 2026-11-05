import { Card, Typography, Divider } from "antd";
import {
  InfoCircleOutlined,
  ExperimentOutlined,
  CloudServerOutlined,
  HeartOutlined,
  TeamOutlined,
} from "@ant-design/icons";

const { Title, Paragraph } = Typography;

// Ícone no verde do tema, dentro de um círculo verde-fraco (mesmo estilo do avatar da Clínica)
const IconeDestaque = ({ children }) => (
  <span className="flex items-center justify-center w-11 h-11 mb-3 rounded-full bg-verde-fraco text-verde-medio text-xl">
    {children}
  </span>
);

const DESTAQUES = [
  {
    icone: <ExperimentOutlined />,
    titulo: "Tecnologia e Ciência",
    texto: (
      <>
        Utilizando os modelos do NRC (2006), o sistema estima a{" "}
        <strong>energia metabolizável</strong> de alimentos industrializados a partir das
        informações bromatológicas dos rótulos, recomendando de forma precisa a quantidade diária
        ideal de alimento.
      </>
    ),
  },
  {
    icone: <HeartOutlined />,
    titulo: "Foco no Bem-Estar Animal",
    texto: (
      <>
        As recomendações nutricionais são <strong>individualizadas</strong> com base em dados como
        espécie, peso, sexo, idade, nível de atividade e estado fisiológico (como gestação ou
        lactação).
      </>
    ),
  },
  {
    icone: <CloudServerOutlined />,
    titulo: "Arquitetura Moderna",
    texto: (
      <>
        O sistema está sendo desenvolvido com API própria (backend), interface intuitiva
        (frontend) e integração via Docker em servidor da LaMCAD/UFG, garantindo escalabilidade e
        desempenho.
      </>
    ),
  },
  {
    icone: <TeamOutlined />,
    titulo: "Equipe Multidisciplinar",
    texto: (
      <>
        O projeto reúne profissionais de <strong>Medicina Veterinária</strong>,{" "}
        <strong>Zootecnia</strong> e <strong>Ciência da Computação</strong>, integrando saberes
        para um sistema robusto e confiável.
      </>
    ),
  },
];

const PaginaSobre = () => {
  return (
    <div className="flex-1 flex justify-center px-4 py-8 md:px-8">
      <Card className="w-full max-w-5xl h-fit shadow-sm">
        <Title level={2} className="text-center text-verde-medio">
          <InfoCircleOutlined className="mr-2" />
          Sobre o Projeto
        </Title>

        <Paragraph className="text-base leading-relaxed">
          Este software tem como objetivo auxiliar no controle nutricional de cães e gatos,
          utilizando o <strong>sistema de regras de Laflamme</strong> para padronizar a avaliação do
          Escore de Condição Corporal (ECC) e as <strong>equações metabólicas do NRC</strong> para o
          cálculo dietético. Com isso, o sistema oferece rigor científico e contribui diretamente
          para a promoção da saúde e do bem-estar dos pets.
        </Paragraph>

        <Divider />

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {DESTAQUES.map((destaque) => (
            <Card key={destaque.titulo} className="h-full">
              <IconeDestaque>{destaque.icone}</IconeDestaque>
              <Title level={4} className="text-verde-escuro">
                {destaque.titulo}
              </Title>
              <Paragraph className="mb-0">{destaque.texto}</Paragraph>
            </Card>
          ))}
        </div>
      </Card>
    </div>
  );
};

export default PaginaSobre;
