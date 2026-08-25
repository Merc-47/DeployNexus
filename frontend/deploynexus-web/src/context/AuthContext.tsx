import {
    createContext,
    useContext,
    useEffect,
    useState,
} from "react";

import {
    getCurrentUser,
    login as loginApi,
} from "../services/api/authApi";

import type {
    LoginRequest,
    LoginResponse,
} from "../types/auth";

import type {
    CurrentUser,
} from "../services/api/authApi";


interface AuthContextType {

    user: CurrentUser | null;

    token: string | null;

    isAuthenticated: boolean;

    isLoading: boolean;

    login: (
        request: LoginRequest
    ) => Promise<LoginResponse>;

    logout: () => void;
}


const AuthContext =
    createContext<AuthContextType | undefined>(
        undefined
    );


export function AuthProvider({
    children,
}: {
    children: React.ReactNode;
}) {

    const [user, setUser] =
        useState<CurrentUser | null>(null);

    const [token, setToken] =
        useState<string | null>(
            localStorage.getItem(
                "deploynexus_token"
            )
        );

    const [isLoading, setIsLoading] =
        useState(true);


    // ========================================================
    // Restore authentication on application startup
    // ========================================================

    useEffect(() => {

        async function restoreAuthentication() {

            const storedToken =
                localStorage.getItem(
                    "deploynexus_token"
                );

            if (!storedToken) {
                setIsLoading(false);
                return;
            }


            try {

                const currentUser =
                    await getCurrentUser();

                setToken(storedToken);

                setUser(currentUser);

            } catch (error) {

                console.error(
                    "Failed to restore authentication:",
                    error
                );

                localStorage.removeItem(
                    "deploynexus_token"
                );

                localStorage.removeItem(
                    "deploynexus_token_expires"
                );

                setToken(null);
                setUser(null);

            } finally {

                setIsLoading(false);

            }
        }


        restoreAuthentication();

    }, []);


    // ========================================================
    // Login
    // ========================================================

    async function login(
        request: LoginRequest
    ): Promise<LoginResponse> {

        const response =
            await loginApi(request);


        localStorage.setItem(
            "deploynexus_token",
            response.token
        );


        localStorage.setItem(
            "deploynexus_token_expires",
            response.expiresAt
        );


        setToken(response.token);


        const currentUser =
            await getCurrentUser();


        setUser(currentUser);


        return response;
    }


    // ========================================================
    // Logout
    // ========================================================

    function logout() {

        localStorage.removeItem(
            "deploynexus_token"
        );

        localStorage.removeItem(
            "deploynexus_token_expires"
        );

        setToken(null);

        setUser(null);
    }


    return (
        <AuthContext.Provider
            value={{
                user,

                token,

                isAuthenticated:
                    !!token && !!user,

                isLoading,

                login,

                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}


// ============================================================
// Hook
// ============================================================

export function useAuth() {

    const context =
        useContext(AuthContext);


    if (!context) {

        throw new Error(
            "useAuth must be used inside AuthProvider"
        );

    }


    return context;
}