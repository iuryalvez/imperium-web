import React from "react";

/**
 * IconeMascara
 * Pinta um ícone PNG dos assets com uma cor do tema, usando o PNG como máscara CSS
 * (os PNGs do projeto são pretos ou brancos e não mudam de cor sozinhos).
 *
 * Props:
 * - src: o PNG importado de assets/.
 * - tamanho: classes de largura/altura do Tailwind (padrão "w-5 h-5").
 * - cor: classe de fundo que vira a cor do ícone (padrão verde-médio).
 */
const IconeMascara = ({ src, tamanho = "w-5 h-5", cor = "bg-verde-medio" }) => (
  <span
    aria-hidden="true"
    className={`inline-block shrink-0 ${tamanho} ${cor}`}
    style={{
      WebkitMask: `url(${src}) center/contain no-repeat`,
      mask: `url(${src}) center/contain no-repeat`,
    }}
  />
);

export default IconeMascara;
