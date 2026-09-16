import React from "react";
import { colors } from "../../../../../../../theme/colors";

const FormulaPreview = ({
  tipoDieta,
  isGato,
  isCustomizado,
  multiplicadorCustom,
  potenciaCustom,
  pesoAtual,
  pesoObjetivo,
  perfilMetabolicoLabel,
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

  const kFinal = isCustomizado
    ? (multiplicadorCustom !== undefined && multiplicadorCustom !== null ? multiplicadorCustom : "K")
    : defaultK;
  
  const pFinal = isCustomizado
    ? (potenciaCustom !== undefined && potenciaCustom !== null ? potenciaCustom : "P")
    : defaultP;

  const pesoFormatado = pesoAtual ? `${pesoAtual}`.replace(".", ",") : "Peso";

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
    
    // Manutenção e Emagrecimento (1 e 5)
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
    </div>
  );
};

export default FormulaPreview;
