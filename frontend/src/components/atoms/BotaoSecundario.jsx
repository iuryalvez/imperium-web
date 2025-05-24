import React from 'react';

const BotaoSecundario = ({ texto, onClick, type = 'button' }) => {
  return (
    <div>
      <button
        type={type}
        onClick={onClick}
        className="bg-verde-medio hover:bg-verde-realce text-white font-bold px-10 rounded-lg h-10 transition-all duration-300"
      >
        {texto}
      </button>
    </div>
  );
};

export default BotaoSecundario;