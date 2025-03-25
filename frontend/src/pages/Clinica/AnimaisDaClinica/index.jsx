import "../../../App.css";
import "../../../components/styles/Listas.css";

import Navsidebar from "../../../components/organisms/Navsidebar";

function AnimaisDaClinica() {
  return (
    <div>
      <Navsidebar></Navsidebar>
      <main className="main-content">
        <div className="titulo-animais">
          <h2>Lista de animais</h2>
        </div>
      </main>
    </div>
  );
}

export default AnimaisDaClinica;
