import { useState } from "react";
import { Outlet, NavLink } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

export default function AppLayout() {

    const { user, logout } = useAuth();

    const [sidebarOpen, setSidebarOpen] =
        useState(true);

    const handleLogout = () => {
        logout();
    };

    return (
        <div className="app-layout">

            {/* =====================================================
                SIDEBAR
            ===================================================== */}

            <aside
                className={`app-sidebar ${
                    sidebarOpen
                        ? "open"
                        : "collapsed"
                }`}
            >

                {/* Logo */}

                <div className="sidebar-header">

                    <div className="sidebar-logo">
                        ⚡
                    </div>

                    {sidebarOpen && (
                        <div className="sidebar-brand">

                            <span className="sidebar-title">
                                Deploy Nexus
                            </span>

                            <span className="sidebar-subtitle">
                                Administration
                            </span>

                        </div>
                    )}

                </div>


                {/* Navigation */}

                <nav className="sidebar-navigation">

                    <NavLink
                        to="/dashboard"
                        className={({ isActive }) =>
                            `sidebar-link ${
                                isActive
                                    ? "active"
                                    : ""
                            }`
                        }
                    >
                        <span className="sidebar-icon">
                            ▣
                        </span>

                        {sidebarOpen && (
                            <span>
                                Dashboard
                            </span>
                        )}
                    </NavLink>


                    <NavLink
                        to="/users"
                        className={({ isActive }) =>
                            `sidebar-link ${
                                isActive
                                    ? "active"
                                    : ""
                            }`
                        }
                    >
                        <span className="sidebar-icon">
                            👥
                        </span>

                        {sidebarOpen && (
                            <span>
                                Users
                            </span>
                        )}
                    </NavLink>


                    <NavLink
                        to="/organizations"
                        className={({ isActive }) =>
                            `sidebar-link ${
                                isActive
                                    ? "active"
                                    : ""
                            }`
                        }
                    >
                        <span className="sidebar-icon">
                            🏢
                        </span>

                        {sidebarOpen && (
                            <span>
                                Organizations
                            </span>
                        )}
                    </NavLink>


                    <NavLink
                        to="/roles"
                        className={({ isActive }) =>
                            `sidebar-link ${
                                isActive
                                    ? "active"
                                    : ""
                            }`
                        }
                    >
                        <span className="sidebar-icon">
                            🛡
                        </span>

                        {sidebarOpen && (
                            <span>
                                Roles
                            </span>
                        )}
                    </NavLink>


                    <NavLink
                        to="/permissions"
                        className={({ isActive }) =>
                            `sidebar-link ${
                                isActive
                                    ? "active"
                                    : ""
                            }`
                        }
                    >
                        <span className="sidebar-icon">
                            🔑
                        </span>

                        {sidebarOpen && (
                            <span>
                                Permissions
                            </span>
                        )}
                    </NavLink>


                    <NavLink
                        to="/modules"
                        className={({ isActive }) =>
                            `sidebar-link ${
                                isActive
                                    ? "active"
                                    : ""
                            }`
                        }
                    >
                        <span className="sidebar-icon">
                            ◈
                        </span>

                        {sidebarOpen && (
                            <span>
                                Modules
                            </span>
                        )}
                    </NavLink>

                </nav>


                {/* Footer */}

                <div className="sidebar-footer">

                    <button
                        type="button"
                        className="sidebar-link logout-button"
                        onClick={handleLogout}
                    >

                        <span className="sidebar-icon">
                            ↪
                        </span>

                        {sidebarOpen && (
                            <span>
                                Logout
                            </span>
                        )}

                    </button>

                </div>

            </aside>


            {/* =====================================================
                MAIN APPLICATION
            ===================================================== */}

            <div
                className={`app-main ${
                    sidebarOpen
                        ? "sidebar-open"
                        : "sidebar-collapsed"
                }`}
            >

                {/* Topbar */}

                <header className="app-topbar">

                    <div className="topbar-left">

                        <button
                            type="button"
                            className="sidebar-toggle"
                            onClick={() =>
                                setSidebarOpen(
                                    !sidebarOpen
                                )
                            }
                        >
                            ☰
                        </button>

                        <div className="topbar-title">
                            Deploy Nexus
                        </div>

                    </div>


                    {/* User */}

                    <div className="topbar-user">

                        <div className="user-avatar">
                            {(
                                user?.firstName ||
                                user?.username ||
                                "U"
                            ).charAt(0).toUpperCase()}
                        </div>

                        <div className="user-details">

                            <span className="user-name">
                                {user?.firstName ||
                                    user?.username ||
                                    "User"}
                            </span>

                            <span className="user-email">
                                {user?.email || ""}
                            </span>

                        </div>

                    </div>

                </header>


                {/* Page */}

                <main className="app-content">
                    <Outlet />
                </main>

            </div>

        </div>
    );
}