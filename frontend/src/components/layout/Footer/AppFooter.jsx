import React from "react";
import { Layout, Tooltip } from "antd";
import logoUfg from "../../../assets/UFG_logo.png";
import logoEvz from "../../../assets/EVZ_UFG.png";
import logoInf from "../../../assets/INF_UFG.png";
import logoSps from "../../../assets/sps-logo-preto.png";
import "./AppFooter.css";

const { Footer } = Layout;

// Ano em que o desenvolvimento do Imperium começou (primeiro commit do repositório).
const ANO_INICIO = 2023;

const ORGANIZACOES = [
  { nome: "Universidade Federal de Goiás (UFG)", logo: logoUfg },
  { nome: "Escola de Veterinária e Zootecnia (EVZ/UFG)", logo: logoEvz },
  { nome: "Instituto de Informática (INF/UFG)", logo: logoInf },
  { nome: "Setor de Pecuária Sustentável (SPS)", logo: logoSps },
];

/**
 * AppFooter
 * Rodapé fino das telas logadas: nome do sistema, período de desenvolvimento e as organizações
 * responsáveis. Sem props.
 */
const AppFooter = () => {
  const anoAtual = new Date().getFullYear();
  const periodo = anoAtual > ANO_INICIO ? `${ANO_INICIO}–${anoAtual}` : `${ANO_INICIO}`;

  return (
    <Footer className="app-footer">
      <span className="app-footer-texto">
        <strong>IMPERIUM</strong> © {periodo} · Software Rural
      </span>
      <span className="app-footer-organizacoes">
        <span className="app-footer-texto">Desenvolvido por</span>
        {ORGANIZACOES.map(({ nome, logo }) => (
          <Tooltip key={nome} title={nome}>
            <img src={logo} alt={nome} className="app-footer-logo" />
          </Tooltip>
        ))}
      </span>
    </Footer>
  );
};

export default AppFooter;
