import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function ProtectedRoute() {

    const {
        isAuthenticated,
        isLoading,
    } = useAuth();


    if (isLoading) {

        return (
            <div className="loading-screen">
                <div className="loading-spinner"></div>

                <p>
                    Loading Deploy Nexus...
                </p>
            </div>
        );
    }


    if (!isAuthenticated) {

        return (
            <Navigate
                to="/login"
                replace
            />
        );
    }


    return <Outlet />;
}