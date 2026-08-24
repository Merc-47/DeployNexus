import {
    useEffect,
    useMemo,
    useState,
} from "react";

import PermissionForm from "./PermissionForm";

import {
    createPermission,
    deactivatePermission,
    getPermissions,
    updatePermission,
} from "../../services/api/permissionsApi";

import type {
    Permission,
    CreatePermissionRequest,
    UpdatePermissionRequest,
} from "../../types/permissions";


export default function PermissionsPage() {


    // ============================================================
    // State
    // ============================================================

    const [permissions, setPermissions] =
        useState<Permission[]>([]);


    const [loading, setLoading] =
        useState(true);


    const [formLoading, setFormLoading] =
        useState(false);


    const [error, setError] =
        useState("");


    const [showForm, setShowForm] =
        useState(false);


    const [editingPermission, setEditingPermission] =
        useState<Permission | null>(null);


    const [searchTerm, setSearchTerm] =
        useState("");


    const [statusFilter, setStatusFilter] =
        useState("all");


    // ============================================================
    // Load data
    // ============================================================

    async function loadPermissions() {

        try {

            setLoading(true);

            setError("");


            const data =
                await getPermissions();


            setPermissions(data);

        } catch (err) {

            console.error(
                "Failed to load permissions:",
                err
            );


            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to load permissions."
            );

        } finally {

            setLoading(false);

        }
    }


    useEffect(() => {

        loadPermissions();

    }, []);


    // ============================================================
    // Statistics
    // ============================================================

    const totalPermissions =
        permissions.length;


    const activePermissions =
        permissions.filter(
            permission =>
                permission.isActive
        ).length;


    const inactivePermissions =
        permissions.filter(
            permission =>
                !permission.isActive
        ).length;


    // ============================================================
    // Filter permissions
    // ============================================================

    const filteredPermissions =
        useMemo(() => {

            const search =
                searchTerm
                    .trim()
                    .toLowerCase();


            return permissions.filter(
                permission => {

                    const matchesSearch =
                        !search ||
                        permission.name
                            .toLowerCase()
                            .includes(search) ||
                        permission.code
                            .toLowerCase()
                            .includes(search) ||
                        permission.moduleId
                            .toLowerCase()
                            .includes(search);


                    const matchesStatus =
                        statusFilter === "all" ||
                        (
                            statusFilter === "active" &&
                            permission.isActive
                        ) ||
                        (
                            statusFilter === "inactive" &&
                            !permission.isActive
                        );


                    return (
                        matchesSearch &&
                        matchesStatus
                    );

                }
            );

        }, [
            permissions,
            searchTerm,
            statusFilter,
        ]);


    // ============================================================
    // Create
    // ============================================================

    function handleCreate() {

        setEditingPermission(null);

        setShowForm(true);

        setError("");

    }


    // ============================================================
    // Edit
    // ============================================================

    function handleEdit(
        permission: Permission
    ) {

        setEditingPermission(
            permission
        );

        setShowForm(true);

        setError("");

    }


    // ============================================================
    // Submit
    // ============================================================

    async function handleSubmit(
        data:
            | CreatePermissionRequest
            | UpdatePermissionRequest
    ) {

        try {

            setFormLoading(true);

            setError("");


            if (editingPermission) {

                const updatedPermission =
                    await updatePermission(
                        editingPermission.id,
                        data as UpdatePermissionRequest
                    );


                setPermissions(
                    current =>
                        current.map(
                            permission =>
                                permission.id ===
                                updatedPermission.id
                                    ? updatedPermission
                                    : permission
                        )
                );

            } else {

                const newPermission =
                    await createPermission(
                        data as CreatePermissionRequest
                    );


                setPermissions(
                    current => [
                        ...current,
                        newPermission,
                    ]
                );

            }


            setShowForm(false);

            setEditingPermission(null);

        } catch (err) {

            console.error(
                "Failed to save permission:",
                err
            );


            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to save permission."
            );


            throw err;

        } finally {

            setFormLoading(false);

        }

    }


    // ============================================================
    // Deactivate
    // ============================================================

    async function handleDeactivate(
        permission: Permission
    ) {

        const confirmed =
            window.confirm(
                `Are you sure you want to deactivate "${permission.name}"?`
            );


        if (!confirmed) {

            return;

        }


        try {

            await deactivatePermission(
                permission.id
            );


            setPermissions(
                current =>
                    current.map(
                        item =>
                            item.id === permission.id
                                ? {
                                    ...item,
                                    isActive: false,
                                }
                                : item
                    )
            );

        } catch (err) {

            console.error(
                "Failed to deactivate permission:",
                err
            );


            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to deactivate permission."
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

                    <div className="roles-spinner">
                    </div>

                    <span>
                        Loading permissions...
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

                        <span>
                            /
                        </span>

                        Permissions

                    </div>


                    <h1>
                        Permissions
                    </h1>


                    <p>
                        Manage system permissions and
                        control access capabilities.
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

                    Create Permission

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
                        🔑
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Total Permissions
                        </span>


                        <strong>
                            {totalPermissions}
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
                            {activePermissions}
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
                            {inactivePermissions}
                        </strong>

                    </div>

                </div>

            </div>


            {/* ====================================================
                Content Card
            ==================================================== */}

            <div className="roles-content-card">


                {/* =================================================
                    Toolbar
                ================================================= */}

                <div className="roles-toolbar">


                    <div className="roles-search">

                        <span>
                            🔍
                        </span>


                        <input
                            type="text"
                            placeholder="Search permissions..."
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
                                All permissions
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

                {filteredPermissions.length === 0 ? (

                    <div className="roles-empty">

                        <div className="roles-empty-icon">
                            🔑
                        </div>


                        <h2>
                            No permissions found
                        </h2>


                        <p>

                            {permissions.length === 0
                                ? "Create your first permission to get started."
                                : "Try adjusting your search or filters."
                            }

                        </p>


                        {permissions.length === 0 && (

                            <button
                                type="button"
                                className="primary-button"
                                onClick={handleCreate}
                            >

                                + Create Permission

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
                                            PERMISSION
                                        </th>


                                        <th>
                                            CODE
                                        </th>


                                        <th>
                                            MODULE
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

                                    {filteredPermissions.map(
                                        permission => (

                                            <tr
                                                key={
                                                    permission.id
                                                }
                                            >


                                                {/* Permission */}

                                                <td>

                                                    <div className="role-name-cell">

                                                        <div className="role-avatar">
                                                            {permission.name
                                                                .charAt(0)
                                                                .toUpperCase()}
                                                        </div>


                                                        <div className="role-name-info">

                                                            <strong>
                                                                {permission.name}
                                                            </strong>


                                                            <span>
                                                                Permission
                                                            </span>

                                                        </div>

                                                    </div>

                                                </td>


                                                {/* Code */}

                                                <td>

                                                    <span className="role-description">

                                                        {permission.code}

                                                    </span>

                                                </td>


                                                {/* Module */}

                                                <td>

                                                    <div className="organization-cell">

                                                        <span className="organization-icon">
                                                            🧩
                                                        </span>


                                                        <span>

                                                            {permission.moduleId}

                                                        </span>

                                                    </div>

                                                </td>


                                                {/* Status */}

                                                <td>

                                                    <span
                                                        className={
                                                            permission.isActive
                                                                ? "role-status active"
                                                                : "role-status inactive"
                                                        }
                                                    >

                                                        <span className="status-dot">
                                                        </span>


                                                        {permission.isActive
                                                            ? "Active"
                                                            : "Inactive"}

                                                    </span>

                                                </td>


                                                {/* Actions */}

                                                <td>

                                                    <div className="role-actions">


                                                        <button
                                                            type="button"
                                                            className="table-action edit"
                                                            onClick={() =>
                                                                handleEdit(
                                                                    permission
                                                                )
                                                            }
                                                            disabled={
                                                                !permission.isActive
                                                            }
                                                        >
                                                            Edit
                                                        </button>


                                                        <button
                                                            type="button"
                                                            className="table-action deactivate"
                                                            onClick={() =>
                                                                handleDeactivate(
                                                                    permission
                                                                )
                                                            }
                                                            disabled={
                                                                !permission.isActive
                                                            }
                                                        >
                                                            Deactivate
                                                        </button>

                                                    </div>

                                                </td>

                                            </tr>

                                        )
                                    )}

                                </tbody>

                            </table>

                        </div>


                        {/* =================================================
                            Footer
                        ================================================= */}

                        <div className="roles-table-footer">

                            <span>

                                Showing{" "}

                                <strong>
                                    {filteredPermissions.length}
                                </strong>{" "}

                                of{" "}

                                <strong>
                                    {permissions.length}
                                </strong>{" "}

                                permissions

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

                        <PermissionForm

                            permission={
                                editingPermission
                            }

                            onSubmit={
                                handleSubmit
                            }

                            onCancel={() => {

                                setShowForm(false);

                                setEditingPermission(null);

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