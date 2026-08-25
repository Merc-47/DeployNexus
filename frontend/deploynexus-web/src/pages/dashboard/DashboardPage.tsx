import { useEffect, useState } from "react";
import { apiClient } from "../../services/api/apiClient";
import { useAuth } from "../../context/AuthContext";


interface DashboardSummary {
    users: number;
    organizations: number;
    roles: number;
    permissions: number;
    modules: number;
}


export default function DashboardPage() {

    const { user } = useAuth();

    const [summary, setSummary] =
        useState<DashboardSummary | null>(null);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);


    // ============================================================
    // Load Dashboard Summary
    // ============================================================

    useEffect(() => {

        async function loadDashboardSummary() {

            try {

                setLoading(true);
                setError(null);


                const data =
                    await apiClient<DashboardSummary>(
                        "/api/Dashboard/summary"
                    );


                setSummary(data);

            } catch (err) {

                console.error(
                    "Failed to load dashboard summary:",
                    err
                );


                if (err instanceof Error) {

                    setError(err.message);

                } else {

                    setError(
                        "Unable to load dashboard information."
                    );

                }

            } finally {

                setLoading(false);

            }
        }


        loadDashboardSummary();

    }, []);


    // ============================================================
    // Loading
    // ============================================================

    if (loading) {

        return (

            <div className="dashboard-page">

                <div className="dashboard-loading">

                    <div className="dashboard-spinner"></div>

                    <p>
                        Loading dashboard...
                    </p>

                </div>

            </div>

        );
    }


    // ============================================================
    // Error
    // ============================================================

    if (error) {

        return (

            <div className="dashboard-page">

                <div className="dashboard-error">

                    <h2>
                        Unable to load dashboard
                    </h2>

                    <p>
                        {error}
                    </p>

                </div>

            </div>

        );
    }


    // ============================================================
    // Dashboard
    // ============================================================

    return (

        <div className="dashboard-page">


            {/* =====================================================
                Header
            ====================================================== */}

            <div className="dashboard-header">

                <div>

                    <p className="dashboard-eyebrow">
                        OVERVIEW
                    </p>

                    <h1>
                        Welcome back
                        {user?.username
                            ? `, ${user.username}`
                            : ""}
                    </h1>

                    <p className="dashboard-subtitle">

                        Here's an overview of your
                        Deploy Nexus platform.

                    </p>

                </div>


                <div className="dashboard-date">

                    <span>
                        System Overview
                    </span>

                    <strong>
                        Deploy Nexus
                    </strong>

                </div>

            </div>


            {/* =====================================================
                Statistics Cards
            ====================================================== */}

            <div className="dashboard-stats">


                {/* Users */}

                <div className="dashboard-stat-card">

                    <div className="stat-card-top">

                        <div className="stat-icon stat-users">
                            👥
                        </div>

                        <span className="stat-label">
                            USERS
                        </span>

                    </div>


                    <div className="stat-value">

                        {summary?.users ?? 0}

                    </div>


                    <div className="stat-description">

                        Registered users in the system

                    </div>

                </div>


                {/* Organizations */}

                <div className="dashboard-stat-card">

                    <div className="stat-card-top">

                        <div className="stat-icon stat-organizations">
                            🏢
                        </div>

                        <span className="stat-label">
                            ORGANIZATIONS
                        </span>

                    </div>


                    <div className="stat-value">

                        {summary?.organizations ?? 0}

                    </div>


                    <div className="stat-description">

                        Organizations currently managed

                    </div>

                </div>


                {/* Roles */}

                <div className="dashboard-stat-card">

                    <div className="stat-card-top">

                        <div className="stat-icon stat-roles">
                            🛡
                        </div>

                        <span className="stat-label">
                            ROLES
                        </span>

                    </div>


                    <div className="stat-value">

                        {summary?.roles ?? 0}

                    </div>


                    <div className="stat-description">

                        Configured system roles

                    </div>

                </div>


                {/* Permissions */}

                <div className="dashboard-stat-card">

                    <div className="stat-card-top">

                        <div className="stat-icon stat-permissions">
                            🔑
                        </div>

                        <span className="stat-label">
                            PERMISSIONS
                        </span>

                    </div>


                    <div className="stat-value">

                        {summary?.permissions ?? 0}

                    </div>


                    <div className="stat-description">

                        Available access permissions

                    </div>

                </div>

            </div>


            {/* =====================================================
                Bottom Section
            ====================================================== */}

            <div className="dashboard-grid">


                {/* System Overview */}

                <div className="dashboard-panel">

                    <div className="dashboard-panel-header">

                        <div>

                            <p className="panel-eyebrow">
                                PLATFORM
                            </p>

                            <h2>
                                System Overview
                            </h2>

                        </div>

                    </div>


                    <div className="overview-list">


                        <div className="overview-item">

                            <div className="overview-item-left">

                                <div className="overview-icon">
                                    📦
                                </div>

                                <div>

                                    <strong>
                                        Modules
                                    </strong>

                                    <span>
                                        Available system modules
                                    </span>

                                </div>

                            </div>


                            <div className="overview-value">

                                {summary?.modules ?? 0}

                            </div>

                        </div>


                        <div className="overview-item">

                            <div className="overview-item-left">

                                <div className="overview-icon">
                                    🔐
                                </div>

                                <div>

                                    <strong>
                                        Security
                                    </strong>

                                    <span>
                                        Role and permission management
                                    </span>

                                </div>

                            </div>


                            <div className="overview-status active">

                                Active

                            </div>

                        </div>


                        <div className="overview-item">

                            <div className="overview-item-left">

                                <div className="overview-icon">
                                    ⚡
                                </div>

                                <div>

                                    <strong>
                                        API Connection
                                    </strong>

                                    <span>
                                        Backend service status
                                    </span>

                                </div>

                            </div>


                            <div className="overview-status active">

                                Connected

                            </div>

                        </div>

                    </div>

                </div>


                {/* Quick Summary */}

                <div className="dashboard-panel">

                    <div className="dashboard-panel-header">

                        <div>

                            <p className="panel-eyebrow">
                                MANAGEMENT
                            </p>

                            <h2>
                                Quick Summary
                            </h2>

                        </div>

                    </div>


                    <div className="quick-summary">


                        <div className="summary-row">

                            <span>
                                Total Users
                            </span>

                            <strong>
                                {summary?.users ?? 0}
                            </strong>

                        </div>


                        <div className="summary-row">

                            <span>
                                Organizations
                            </span>

                            <strong>
                                {summary?.organizations ?? 0}
                            </strong>

                        </div>


                        <div className="summary-row">

                            <span>
                                Roles
                            </span>

                            <strong>
                                {summary?.roles ?? 0}
                            </strong>

                        </div>


                        <div className="summary-row">

                            <span>
                                Permissions
                            </span>

                            <strong>
                                {summary?.permissions ?? 0}
                            </strong>

                        </div>


                        <div className="summary-row">

                            <span>
                                Modules
                            </span>

                            <strong>
                                {summary?.modules ?? 0}
                            </strong>

                        </div>

                    </div>


                    <div className="dashboard-system-status">

                        <div className="status-indicator">

                            <span className="status-dot"></span>

                            <span>
                                All systems operational
                            </span>

                        </div>

                    </div>

                </div>

            </div>


        </div>

    );
}