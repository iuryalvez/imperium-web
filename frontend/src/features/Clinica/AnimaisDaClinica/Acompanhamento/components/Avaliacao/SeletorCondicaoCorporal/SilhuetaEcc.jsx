import React, { useMemo } from "react";
import { geometriaSilhueta } from "./geometriaSilhuetas";

/**
 * Silhueta do animal (vista de cima e lateral) num escore de condição corporal.
 * Desenhada em `currentColor`: a cor vem do elemento pai.
 */
const SilhuetaEcc = ({ especie, escore, className }) => {
  const formas = useMemo(() => geometriaSilhueta(especie, escore), [especie, escore]);

  return (
    <svg
      viewBox="0 0 120 100"
      className={className}
      aria-hidden="true"
      focusable="false"
      fill="currentColor"
      stroke="currentColor"
      strokeLinecap="round"
      strokeLinejoin="round"
    >
      {formas.pernasFundo.map((d) => (
        <path key={d} d={d} fill="none" strokeWidth={formas.larguraPerna} strokeOpacity={0.55} />
      ))}
      {formas.tracos.map((traco) => (
        <path key={traco.d} d={traco.d} fill="none" strokeWidth={traco.largura} />
      ))}
      {formas.preenchidos.map((d) => (
        <path key={d} d={d} stroke="none" />
      ))}
      {formas.pernasFrente.map((d) => (
        <path key={d} d={d} fill="none" strokeWidth={formas.larguraPerna} />
      ))}
    </svg>
  );
};

export default SilhuetaEcc;
