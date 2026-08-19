import { useState } from "react";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

export default function LoginPage() {
    const navigate = useNavigate();

    const { login } = useAuth();

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        setError("");

        if (!username.trim() || !password) {
            setError(
                "Please enter your username and password."
            );

            return;
        }

        try {
            setLoading(true);

            // AuthContext handles:
            // - API login
            // - JWT storage
            // - token state
            // - current user
            // - authentication state

            await login({
                username: username.trim(),
                password,
            });

            console.log("Login successful");

            // Authentication is now established.
            navigate("/dashboard");

        } catch (err: unknown) {
            console.error(
                "Login failed:",
                err
            );

            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError(
                    "Unable to sign in. Please try again."
                );
            }

        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-page">

            {/* =====================================================
                BACKGROUND
            ====================================================== */}

            <div className="login-background">

                <div className="background-circle circle-one"></div>

                <div className="background-circle circle-two"></div>

                <div className="background-circle circle-three"></div>

            </div>


            <div className="login-container">

                {/* =================================================
                    BRANDING SECTION
                ================================================== */}

                <div className="login-brand">

                    <div className="brand-content">

                        <div className="brand-icon">
                            ⚡
                        </div>

                        <h1>
                            Deploy Nexus
                        </h1>

                        <p className="brand-tagline">
                            Deployment management,
                            simplified.
                        </p>

                        <p className="brand-description">
                            Manage users, organizations,
                            roles, permissions and
                            deployments from one
                            centralized platform.
                        </p>


                        <div className="feature-list">

                            <div className="feature-item">

                                <span className="feature-icon">
                                    ✓
                                </span>

                                <span>
                                    Secure authentication
                                </span>

                            </div>


                            <div className="feature-item">

                                <span className="feature-icon">
                                    ✓
                                </span>

                                <span>
                                    Role-based access control
                                </span>

                            </div>


                            <div className="feature-item">

                                <span className="feature-icon">
                                    ✓
                                </span>

                                <span>
                                    Centralized management
                                </span>

                            </div>

                        </div>

                    </div>

                </div>


                {/* =================================================
                    LOGIN SECTION
                ================================================== */}

                <div className="login-card-wrapper">

                    <div className="login-card">

                        {/* Header */}

                        <div className="login-header">

                            <div className="mobile-brand-icon">
                                ⚡
                            </div>

                            <h2>
                                Welcome back
                            </h2>

                            <p>
                                Sign in to your Deploy Nexus
                                account
                            </p>

                        </div>


                        {/* =================================================
                            ERROR MESSAGE
                        ================================================== */}

                        {error && (

                            <div className="login-error">

                                <span className="error-icon">
                                    !
                                </span>

                                <span>
                                    {error}
                                </span>

                            </div>

                        )}


                        {/* =================================================
                            LOGIN FORM
                        ================================================== */}

                        <form onSubmit={handleSubmit}>

                            {/* Username */}

                            <div className="form-group">

                                <label htmlFor="username">
                                    Username
                                </label>

                                <div className="input-wrapper">

                                    <span className="input-icon">
                                        👤
                                    </span>

                                    <input
                                        id="username"
                                        type="text"
                                        value={username}
                                        onChange={(event) =>
                                            setUsername(
                                                event.target.value
                                            )
                                        }
                                        placeholder="Enter your username"
                                        autoComplete="username"
                                        disabled={loading}
                                    />

                                </div>

                            </div>


                            {/* Password */}

                            <div className="form-group">

                                <div className="password-label-row">

                                    <label htmlFor="password">
                                        Password
                                    </label>

                                </div>


                                <div className="input-wrapper">

                                    <span className="input-icon">
                                        🔒
                                    </span>

                                    <input
                                        id="password"
                                        type={
                                            showPassword
                                                ? "text"
                                                : "password"
                                        }
                                        value={password}
                                        onChange={(event) =>
                                            setPassword(
                                                event.target.value
                                            )
                                        }
                                        placeholder="Enter your password"
                                        autoComplete="current-password"
                                        disabled={loading}
                                    />


                                    <button
                                        type="button"
                                        className="password-toggle"
                                        onClick={() =>
                                            setShowPassword(
                                                (previous) =>
                                                    !previous
                                            )
                                        }
                                        disabled={loading}
                                        aria-label={
                                            showPassword
                                                ? "Hide password"
                                                : "Show password"
                                        }
                                    >

                                        {showPassword
                                            ? "◉"
                                            : "○"}

                                    </button>

                                </div>

                            </div>


                            {/* Submit */}

                            <button
                                type="submit"
                                className="login-button"
                                disabled={loading}
                            >

                                {loading ? (

                                    <>

                                        <span className="spinner"></span>

                                        Signing in...

                                    </>

                                ) : (

                                    <>

                                        Sign in

                                        <span className="button-arrow">
                                            →
                                        </span>

                                    </>

                                )}

                            </button>

                        </form>


                        {/* =================================================
                            FOOTER
                        ================================================== */}

                        <div className="login-footer">

                            <span>
                                Deploy Nexus
                            </span>

                            <span>
                                •
                            </span>

                            <span>
                                Secure access
                            </span>

                        </div>

                    </div>

                </div>

            </div>

        </div>
    );
}
