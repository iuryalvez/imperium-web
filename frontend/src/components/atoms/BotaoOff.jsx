import React from 'react';
import { useNotification } from '../../services/NotificationContext';

const BotaoOff = ({ texto }) => {
    const abrirNotificacao = useNotification();

    return (
        <div>
            <button
                onClick={() =>
                    abrirNotificacao('error', 'Erro de acesso', 'Esta função ainda não está disponível')
                }
                className="bg-gray-400 text-2xl font-bold text-white px-20 h-20 w-50 rounded-md cursor-not-allowed"
            >
                {texto}
            </button>
        </div>
    );
};

export default BotaoOff;
