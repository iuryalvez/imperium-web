const BotaoPrimario = ({ texto, onClick, type = 'button' }) => {
  return (
    <div>
      <button
        type={type}
        onClick={onClick}
        className="bg-[var(--verde-medio)] hover:bg-[var(--verde-realce)] text-white text-l font-bold py-3 rounded-2xl transition-all duration-300 min-w-full disabled:cursor-not-allowed"
      >
        {texto}
      </button>
    </div>
  );
};

export default BotaoPrimario;
