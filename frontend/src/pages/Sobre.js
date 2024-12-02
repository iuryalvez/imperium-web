import React from 'react';

import '../App.css';
import '../components/styles/Sobre.css';

import Logos from '../components/Logos';
import Header from '../components/organisms/Header';

function Sobre() {
  return (
    <div>
      <Header /> 
      <div className='sobre-content'>
        <h2>Imperium canis et feles</h2>
          <p>Lorem ipsum dolor sit amet. Est quia ducimus aut pariatur praesentium et sunt voluptas et aliquam quod et praesentium praesentium. Vel maiores galisum est eaque reprehenderit aut fugit natus ut nulla voluptatem ex doloribus eveniet qui quidem facilis et explicabo rerum. Et nesciunt dolorum aut excepturi omnis ut possimus eligendi aut ullam aspernatur 33 quidem ipsum et saepe reiciendis At cumque nihil. Est quam itaque hic omnis blanditiis et dignissimos quod et corporis obcaecati. </p>
          <p>Est voluptas harum sit repellendus dolores et distinctio assumenda ab omnis fugit nam officia sunt ea repellat distinctio et repellat delectus. Vel vitae iure eum harum aliquam est ducimus sunt et vero nihil. Rem ipsum impedit aut voluptas omnis ad quia dolores in unde quas. Aut officia delectus sed pariatur odio in laborum excepturi ut omnis doloremque aut officia quis. </p>
        <Logos />
      </div>
    </div>
  );
}

export default Sobre;
