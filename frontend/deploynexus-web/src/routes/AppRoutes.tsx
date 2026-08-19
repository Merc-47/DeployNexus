import {
    Navigate,
    Route,
    Routes,
} from "react-router-dom";

import LoginPage from "../pages/auth/LoginPage";
import DashboardPage from "../pages/dashboard/DashboardPage";

import AppLayout from "../components/layout/AppLayout";
import ProtectedRoute from "./ProtectedRoute";


export default function AppRoutes() {

    return (
        <Routes>

            {/* ====================================================
                Public Routes
            ==================================================== */}

            <Route
                path="/login"
                element={<LoginPage />}
            />


            {/* ====================================================
                Protected Routes
            ==================================================== */}

            <Route element={<ProtectedRoute />}>

                <Route
                    element={<AppLayout />}
                >

                    <Route
                        path="/dashboard"
                        element={<DashboardPage />}
                    />

                    <Route
                        path="/users"
                        element={
                            <div>
                                Users
                            </div>
                        }
                    />

                    <Route
                        path="/organizations"
                        element={
                            <div>
                                Organizations
                            </div>
                        }
                    />

                    <Route
                        path="/roles"
                        element={
                            <div>
                                Roles
                            </div>
                        }
                    />

                    <Route
                        path="/permissions"
                        element={
                            <div>
                                Permissions
                            </div>
                        }
                    />

                    <Route
                        path="/modules"
                        element={
                            <div>
                                Modules
                            </div>
                        }
                    />

                </Route>

            </Route>


            {/* ====================================================
                Default
            ==================================================== */}

            <Route
                path="*"
                element={
                    <Navigate
                        to="/dashboard"
                        replace
                    />
                }
            />

        </Routes>
    );
}