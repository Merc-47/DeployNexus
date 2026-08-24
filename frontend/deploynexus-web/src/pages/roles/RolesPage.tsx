import { useEffect, useMemo, useState } from "react";

import RoleForm from "./RoleForm";

import {
    createRole,
    deactivateRole,
    getOrganizations,
    getRoles,
    updateRole,
} from "../../services/api/rolesApi";

import type {
    Role,
    Organization,
} from "../../types/roles";

import { useNavigate } from "react-router-dom";


export default function RolesPage() {

    const navigate = useNavigate();

    const [roles, setRoles] =
        useState<Role[]>([]);

    const [organizations, setOrganizations] =
        useState<Organization[]>([]);

    const [loading, setLoading] =
        useState(true);

    const [formLoading, setFormLoading] =
        useState(false);

    const [error, setError] =
        useState("");

    const [showForm, setShowForm] =
        useState(false);

    const [editingRole, setEditingRole] =
        useState<Role | null>(null);

    const [searchTerm, setSearchTerm] =
        useState("");

    const [statusFilter, setStatusFilter] =
        useState("all");


    // ============================================================
    // Load data
    // ============================================================

    async function loadData() {

        try {

            setLoading(true);
            setError("");

            const [
                rolesData,
                organizationsData,
            ] = await Promise.all([
                getRoles(),
                getOrganizations(),
            ]);

            setRoles(rolesData);

            setOrganizations(
                organizationsData
            );

        } catch (err) {

            console.error(
                "Failed to load roles:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to load roles."
            );

        } finally {

            setLoading(false);

        }
    }


    useEffect(() => {
        loadData();
    }, []);


    // ============================================================
    // Statistics
    // ============================================================

    const totalRoles =
        roles.length;

    const activeRoles =
        roles.filter(
            role => role.isActive
        ).length;

    const inactiveRoles =
        roles.filter(
            role => !role.isActive
        ).length;


    // ============================================================
    // Filter roles
    // ============================================================

    const filteredRoles =
        useMemo(() => {

            const search =
                searchTerm
                    .trim()
                    .toLowerCase();

            return roles.filter(
                role => {

                    const matchesSearch =
                        !search ||
                        role.name
                            .toLowerCase()
                            .includes(search) ||
                        role.description
                            ?.toLowerCase()
                            .includes(search);

                    const matchesStatus =
                        statusFilter === "all" ||
                        (
                            statusFilter === "active" &&
                            role.isActive
                        ) ||
                        (
                            statusFilter === "inactive" &&
                            !role.isActive
                        );

                    return (
                        matchesSearch &&
                        matchesStatus
                    );
                }
            );

        }, [
            roles,
            searchTerm,
            statusFilter,
        ]);


    // ============================================================
    // Create
    // ============================================================

    function handleCreate() {

        setEditingRole(null);
        setShowForm(true);

    }


    // ============================================================
    // Edit
    // ============================================================

    function handleEdit(
        role: Role
    ) {

        setEditingRole(role);
        setShowForm(true);

    }


    // ============================================================
    // Manage Permissions
    // ============================================================

    function handleManagePermissions(
        role: Role
    ) {

        navigate(
            `/roles/${role.id}/permissions`
        );

    }


    // ============================================================
    // Submit
    // ============================================================

    async function handleSubmit(
        name: string,
        description: string,
        organizationId: string
    ) {

        try {

            setFormLoading(true);

            if (editingRole) {

                const updatedRole =
                    await updateRole(
                        editingRole.id,
                        {
                            name,
                            description,
                        }
                    );

                setRoles(
                    current =>
                        current.map(
                            role =>
                                role.id ===
                                updatedRole.id
                                    ? updatedRole
                                    : role
                        )
                );

            } else {

                const newRole =
                    await createRole({
                        name,
                        description,
                        organizationId,
                    });

                setRoles(
                    current => [
                        ...current,
                        newRole,
                    ]
                );
            }

            setShowForm(false);
            setEditingRole(null);

        } finally {

            setFormLoading(false);

        }
    }


    // ============================================================
    // Deactivate
    // ============================================================

    async function handleDeactivate(
        role: Role
    ) {

        const confirmed =
            window.confirm(
                `Are you sure you want to deactivate "${role.name}"?`
            );

        if (!confirmed) {
            return;
        }

        try {

            await deactivateRole(
                role.id
            );

            setRoles(
                current =>
                    current.map(
                        item =>
                            item.id === role.id
                                ? {
                                    ...item,
                                    isActive: false,
                                }
                                : item
                    )
            );

        } catch (err) {

            console.error(
                "Failed to deactivate role:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to deactivate role."
            );
        }
    }


    // ============================================================
    // Loading
    // ============================================================

    if (loading) {

        return (
            <div className="roles-page">

                <div className="roles-loading">

                    <div className="roles-spinner"></div>

                    <span>
                        Loading roles...
                    </span>

                </div>

            </div>
        );
    }


    // ============================================================
    // Page
    // ============================================================

    return (

        <div className="roles-page">

            {/* ====================================================
                Header
            ==================================================== */}

            <div className="roles-page-header">

                <div className="roles-heading">

                    <div className="breadcrumb">
                        Administration
                        <span>/</span>
                        Roles
                    </div>

                    <h1>
                        Roles
                    </h1>

                    <p>
                        Manage roles and control
                        organization access.
                    </p>

                </div>


                <button
                    type="button"
                    className="primary-button"
                    onClick={handleCreate}
                >
                    <span className="button-plus">
                        +
                    </span>

                    Create Role
                </button>

            </div>


            {/* ====================================================
                Error
            ==================================================== */}

            {error && (

                <div className="roles-error">

                    <span className="roles-error-icon">
                        !
                    </span>

                    <span>
                        {error}
                    </span>

                    <button
                        type="button"
                        onClick={() =>
                            setError("")
                        }
                    >
                        ×
                    </button>

                </div>

            )}


            {/* ====================================================
                Statistics
            ==================================================== */}

            <div className="role-statistics">

                <div className="role-stat-card">

                    <div className="role-stat-icon total">
                        🛡
                    </div>

                    <div className="role-stat-content">

                        <span>
                            Total Roles
                        </span>

                        <strong>
                            {totalRoles}
                        </strong>

                    </div>

                </div>


                <div className="role-stat-card">

                    <div className="role-stat-icon active">
                        ✓
                    </div>

                    <div className="role-stat-content">

                        <span>
                            Active
                        </span>

                        <strong>
                            {activeRoles}
                        </strong>

                    </div>

                </div>


                <div className="role-stat-card">

                    <div className="role-stat-icon inactive">
                        −
                    </div>

                    <div className="role-stat-content">

                        <span>
                            Inactive
                        </span>

                        <strong>
                            {inactiveRoles}
                        </strong>

                    </div>

                </div>

            </div>


            {/* ====================================================
                Content Card
            ==================================================== */}

            <div className="roles-content-card">

                {/* Toolbar */}

                <div className="roles-toolbar">

                    <div className="roles-search">

                        <span>
                            🔍
                        </span>

                        <input
                            type="text"
                            placeholder="Search roles..."
                            value={searchTerm}
                            onChange={event =>
                                setSearchTerm(
                                    event.target.value
                                )
                            }
                        />

                    </div>


                    <div className="roles-filter">

                        <label>
                            Status
                        </label>

                        <select
                            value={statusFilter}
                            onChange={event =>
                                setStatusFilter(
                                    event.target.value
                                )
                            }
                        >

                            <option value="all">
                                All roles
                            </option>

                            <option value="active">
                                Active
                            </option>

                            <option value="inactive">
                                Inactive
                            </option>

                        </select>

                    </div>

                </div>


                {/* =================================================
                    Empty state
                ================================================= */}

                {filteredRoles.length === 0 ? (

                    <div className="roles-empty">

                        <div className="roles-empty-icon">
                            🛡
                        </div>

                        <h2>
                            No roles found
                        </h2>

                        <p>
                            {roles.length === 0
                                ? "Create your first role to get started."
                                : "Try adjusting your search or filters."
                            }
                        </p>

                        {roles.length === 0 && (

                            <button
                                type="button"
                                className="primary-button"
                                onClick={handleCreate}
                            >
                                + Create Role
                            </button>

                        )}

                    </div>

                ) : (

                    <>
                        {/* =================================================
                            Table
                        ================================================= */}

                        <div className="roles-table-wrapper">

                            <table className="roles-table">

                                <thead>

                                    <tr>

                                        <th>
                                            ROLE
                                        </th>

                                        <th>
                                            DESCRIPTION
                                        </th>

                                        <th>
                                            ORGANIZATION
                                        </th>

                                        <th>
                                            STATUS
                                        </th>

                                        <th className="actions-header">
                                            ACTIONS
                                        </th>

                                    </tr>

                                </thead>


                                <tbody>

                                    {filteredRoles.map(
                                        role => {

                                            const organization =
                                                organizations.find(
                                                    item =>
                                                        item.id ===
                                                        role.organizationId
                                                );

                                            return (

                                                <tr
                                                    key={
                                                        role.id
                                                    }
                                                >

                                                    {/* Role */}

                                                    <td>

                                                        <div className="role-name-cell">

                                                            <div className="role-avatar">
                                                                {role.name
                                                                    .charAt(0)
                                                                    .toUpperCase()}
                                                            </div>

                                                            <div className="role-name-info">

                                                                <strong>
                                                                    {role.name}
                                                                </strong>

                                                                <span>
                                                                    Role
                                                                </span>

                                                            </div>

                                                        </div>

                                                    </td>


                                                    {/* Description */}

                                                    <td>

                                                        <span className="role-description">

                                                            {role.description ||
                                                                "No description"}

                                                        </span>

                                                    </td>


                                                    {/* Organization */}

                                                    <td>

                                                        <div className="organization-cell">

                                                            <span className="organization-icon">
                                                                🏢
                                                            </span>

                                                            <span>
                                                                {organization
                                                                    ? organization.name
                                                                    : "Unknown organization"}
                                                            </span>

                                                        </div>

                                                    </td>


                                                    {/* Status */}

                                                    <td>

                                                        <span
                                                            className={
                                                                role.isActive
                                                                    ? "role-status active"
                                                                    : "role-status inactive"
                                                            }
                                                        >

                                                            <span className="status-dot">
                                                            </span>

                                                            {role.isActive
                                                                ? "Active"
                                                                : "Inactive"}

                                                        </span>

                                                    </td>


                                                    {/* Actions */}

                                                    <td>

                                                        <div className="role-actions">

                                                            <button
                                                                type="button"
                                                                className="table-action permissions"
                                                                onClick={() =>
                                                                    handleManagePermissions(
                                                                        role
                                                                    )
                                                                }
                                                                disabled={
                                                                    !role.isActive
                                                                }
                                                            >
                                                                Manage Permissions
                                                            </button>


                                                            <button
                                                                type="button"
                                                                className="table-action edit"
                                                                onClick={() =>
                                                                    handleEdit(
                                                                        role
                                                                    )
                                                                }
                                                                disabled={
                                                                    !role.isActive
                                                                }
                                                            >
                                                                Edit
                                                            </button>


                                                            <button
                                                                type="button"
                                                                className="table-action deactivate"
                                                                onClick={() =>
                                                                    handleDeactivate(
                                                                        role
                                                                    )
                                                                }
                                                                disabled={
                                                                    !role.isActive
                                                                }
                                                            >
                                                                Deactivate
                                                            </button>

                                                        </div>

                                                    </td>

                                                </tr>

                                            );

                                        }
                                    )}

                                </tbody>

                            </table>

                        </div>


                        {/* Footer */}

                        <div className="roles-table-footer">

                            <span>
                                Showing{" "}
                                <strong>
                                    {filteredRoles.length}
                                </strong>{" "}
                                of{" "}
                                <strong>
                                    {roles.length}
                                </strong>{" "}
                                roles
                            </span>

                        </div>

                    </>

                )}

            </div>


            {/* ====================================================
                Modal
            ==================================================== */}

            {showForm && (

                <div className="role-modal-overlay">

                    <div className="role-modal">

                        <RoleForm
                            role={
                                editingRole
                            }
                            organizations={
                                organizations
                            }
                            onSubmit={
                                handleSubmit
                            }
                            onCancel={() => {
                                setShowForm(false);
                                setEditingRole(null);
                            }}
                            loading={
                                formLoading
                            }
                        />

                    </div>

                </div>

            )}

        </div>
    );
}