import { Outlet } from "react-router-dom";
import "../css/layout.css";
import { LuHome } from "react-icons/lu";
import { BsFeather } from "react-icons/bs";

const Layout = () => {
    return (
        <div>
            <div className="d-flex flex-row align-items-center justify-content-center">
                <div className="tab">
                    <a href="/" title="Home"><LuHome /></a>
                    <a className="ms-4" href="/editor" title="Draft"><BsFeather /></a>
                </div>
            </div>
            <div>
                <Outlet />
            </div>
        </div>
    );
}

export default Layout;