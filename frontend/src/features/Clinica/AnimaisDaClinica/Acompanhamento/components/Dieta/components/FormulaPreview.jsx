import React from "react";
import { colors } from "../../../../../../../theme/colors";

const FormulaPreview = ({
  tipoDieta,
  isGato,
  multiplicadorCustom,
  potencia,
  pesoAtual,
  pesoObjetivo,
  perfilMetabolicoLabel,
  hasAdicionalGestacaoCao,
  numeroFilhotes,
  semanaLactacao,
  fatorRestricao,
}) => {
  // Parse defaults from perfilMetabolicoLabel (e.g. "Cão Inativo (95 × PC^0,75)")
  let defaultK = "K";
  let defaultP = "P";

  if (perfilMetabolicoLabel) {
    const match = perfilMetabolicoLabel.match(/(\d+(?:,\d+)?)\s*×\s*PC\^(\d+(?:,\d+)?)/);
    if (match) {
      defaultK = match[1];
      defaultP = match[2];
    }
  }

  const kFinal = (multiplicadorCustom !== undefined && multiplicadorCustom !== null) 
    ? multiplicadorCustom 
    : defaultK;
  
  const pFinal = (potencia !== undefined && potencia !== null) 
    ? potencia 
    : defaultP;

  const pesoFormatado = pesoAtual ? `${pesoAtual}`.replace(".", ",") : "Peso";
  const pesoAlvoFormatado = pesoObjetivo ? `${pesoObjetivo}`.replace(".", ",") : "Peso alvo";

  // Calcular valor numérico de p = PCa / PCm, caso tenhamos ambos.
  let pValor = "p";
  let pFootnote = "Onde p = (Peso Atual / Peso Adulto Estimado)";

  if (tipoDieta === 2 && pesoAtual && pesoObjetivo && pesoObjetivo > 0) {
    const pCalc = pesoAtual / pesoObjetivo;
    pValor = pCalc.toFixed(3).replace(".", ",");
    pFootnote = `Onde p = (${pesoFormatado} / ${pesoObjetivo.toString().replace(".", ",")}) = ${pValor}`;
  }

  const renderFormula = () => {
    // Crescimento
    if (tipoDieta === 2) {
      if (isGato) {
        return (
          <>
            {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup> × 6,7 × [e<sup>-0,189({pValor})</sup> - 0,66]
          </>
        );
      }
      return (
        <>
          {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup> × 3,2 × [e<sup>-0,87({pValor})</sup> - 0,1]
        </>
      );
    }
    
    // Lactação (3)
    if (tipoDieta === 3) {
      const lText = "L"; // O usuário verá L e nós sabemos que varia de acordo com a semana
      
      if (isGato) {
        const cte = numeroFilhotes ? (numeroFilhotes < 3 ? 18 : (numeroFilhotes > 4 ? 70 : 60)) : "C";
        return (
          <>
            {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup> + ({cte} × {pesoFormatado}kg × {lText})
          </>
        );
      }
      
      const nForm = (numeroFilhotes && numeroFilhotes <= 4) ? numeroFilhotes : (numeroFilhotes ? 4 : "n");
      const mForm = (numeroFilhotes && numeroFilhotes > 4) ? (numeroFilhotes - 4) : (numeroFilhotes ? 0 : "m");
      return (
        <>
          {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup> + ({pesoFormatado}kg × ((24 × {nForm}) + (12 × {mForm})) × {lText})
        </>
      );
    }

    // Gestação (4): gata usa K × (PC)^P, sem fator de semana
    if (tipoDieta === 4) {
      if (!isGato && hasAdicionalGestacaoCao) {
        return (
          <>
            {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup> + 26 × ({pesoFormatado}kg)
          </>
        );
      }
      return (
        <>
          {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup>
        </>
      );
    }
    
    // Emagrecimento (5)
    if (tipoDieta === 5) {
      const fatorTexto =
        fatorRestricao !== undefined && fatorRestricao !== null
          ? fatorRestricao.toString().replace(".", ",")
          : "R";
      return (
        <>
          [{kFinal} × ({pesoAlvoFormatado}kg)<sup>{pFinal}</sup>] × {fatorTexto}
        </>
      );
    }
    
    // Manutenção e outros
    return (
      <>
        {kFinal} × ({pesoFormatado}kg)<sup>{pFinal}</sup>
      </>
    );
  };

  return (
    <div
      className="p-3 rounded-md mb-4 font-mono text-sm shadow-sm"
      style={{
        backgroundColor: colors.verde.fraco,
        borderColor: colors.verde.claro,
        borderWidth: 1,
        borderStyle: "solid",
        color: colors.verde.escuro,
      }}
    >
      <div className="font-semibold mb-2" style={{ fontSize: "0.8rem", textTransform: "uppercase" }}>
        Fórmula de Cálculo da Necessidade Energética:
      </div>
      <div className="text-base text-center break-words">
        {renderFormula()}
      </div>
      {tipoDieta === 2 && (
        <div className="mt-2 text-xs text-center opacity-80 font-sans">
          * <strong>{pFootnote}</strong>
        </div>
      )}
      {tipoDieta === 3 && (
        <div className="mt-2 text-xs text-center opacity-80 font-sans">
          * <strong>L</strong> é o Fator de Lactação, que varia conforme a semana de lactação ({semanaLactacao || "X"}).
        </div>
      )}
      {tipoDieta === 5 && (
        <div className="mt-2 text-xs text-center opacity-80 font-sans">
          * Energia calculada sobre o <strong>peso alvo</strong>, fixa em todas as semanas até atingi-lo.
        </div>
      )}
    </div>
  );
};

export default FormulaPreview;
