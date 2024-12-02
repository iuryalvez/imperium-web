import React from "react";
import Sidebar from "../molecules/Sidebar";
import Header from "./Header";

function Navsidebar() {
    return (
        <div>
            <header>
                <Header />
            </header>
            <div>
                <Sidebar />
            </div>
        </div>
    )
}

export default Navsidebar;