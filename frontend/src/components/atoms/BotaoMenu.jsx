import React from 'react';
import { Button } from 'antd';

const BotaoMenu = ({ texto, onClick }) => {
  return (
    <div>
      <Button type="primary" onClick={onClick} className="bg-green hover:!bg-green-dark text-2xl font-bold text-white px-20 h-20 w-50">
        {texto}
      </Button>
    </div>
  );
};

export default BotaoMenu;
