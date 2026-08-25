import {
    Navigate,
    Route,
    Routes,
} from "react-router-dom";

import LoginPage from "../pages/auth/LoginPage";
import DashboardPage from "../pages/dashboard/DashboardPage";
import UsersPage from "../pages/users/UsersPage";
import OrganizationsPage from "../pages/organizations/OrganizationsPage";
import RolesPage from "../pages/roles/RolesPage";
import RolePermissionsPage from "../pages/roles/RolePermissionsPage";
import PermissionsPage from "../pages/permissions/PermissionsPage";

import AppLayout from "../components/layout/AppLayout";

import { useAuth } from "../context/AuthContext";


export default function AppRoutes() {

    const {
        isAuthenticated,
        isLoading,
    } = useAuth();


    // ============================================================
    // Restore authentication
    // ============================================================

    if (isLoading) {

        return (
            <div
                style={{
                    minHeight: "100vh",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                }}
            >
                Loading...
            </div>
        );
    }


    return (
        <Routes>


            {/* ====================================================
                PUBLIC ROUTES
            ==================================================== */}

            <Route
                path="/login"
                element={
                    isAuthenticated
                        ? (
                            <Navigate
                                to="/dashboard"
                                replace
                            />
                        )
                        : (
                            <LoginPage />
                        )
                }
            />


            {/* ====================================================
                PROTECTED APPLICATION ROUTES
            ==================================================== */}

            <Route
                element={
                    isAuthenticated
                        ? (
                            <AppLayout />
                        )
                        : (
                            <Navigate
                                to="/login"
                                replace
                            />
                        )
                }
            >


                {/* ==================================================
                    DASHBOARD
                ================================================== */}

                <Route
                    path="/dashboard"
                    element={
                        <DashboardPage />
                    }
                />

                {/* ==================================================
                    USERS
                ================================================== */}

                <Route
                    path="/users"
                    element={
                        <UsersPage />
                    }
                />


                {/* ==================================================
                    ORGANIZATIONS
                ================================================== */}

                <Route
                    path="/organizations"
                    element={
                        <OrganizationsPage />
                    }
                />


                {/* ==================================================
                    ROLES
                ================================================== */}

                <Route
                    path="/roles"
                    element={
                        <RolesPage />
                    }
                />


                {/* ==================================================
                    ROLE PERMISSIONS
                ================================================== */}

                <Route
                    path="/roles/:roleId/permissions"
                    element={
                        <RolePermissionsPage />
                    }
                />


                {/* ==================================================
                    PERMISSIONS
                ================================================== */}

                <Route
                    path="/permissions"
                    element={
                        <PermissionsPage />
                    }
                />


            </Route>


            {/* ====================================================
                DEFAULT ROUTE
            ==================================================== */}

            <Route
                path="/"
                element={
                    <Navigate
                        to={
                            isAuthenticated
                                ? "/dashboard"
                                : "/login"
                        }
                        replace
                    />
                }
            />


            {/* ====================================================
                UNKNOWN ROUTE
            ==================================================== */}

            <Route
                path="*"
                element={
                    <Navigate
                        to={
                            isAuthenticated
                                ? "/dashboard"
                                : "/login"
                        }
                        replace
                    />
                }
            />


        </Routes>
    );
}