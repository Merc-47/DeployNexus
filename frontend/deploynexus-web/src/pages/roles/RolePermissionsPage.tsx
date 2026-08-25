import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import {
    getPermissions,
    getRolePermissions,
    assignRolePermission,
    removeRolePermission,
} from "../../services/api/permissionsApi";

import type { RolePermission } from "../../types/rolePermissions";


export default function RolePermissionsPage() {

    const { roleId } = useParams();

    const navigate = useNavigate();


    // ============================================================
    // State
    // ============================================================

    const [permissions, setPermissions] =
        useState<RolePermission[]>([]);

    const [originalAssignedIds, setOriginalAssignedIds] =
        useState<Set<string>>(new Set());

    const [loading, setLoading] =
        useState(true);

    const [saving, setSaving] =
        useState(false);

    const [error, setError] =
        useState("");

    const [success, setSuccess] =
        useState("");


    // ============================================================
    // Search
    // ============================================================

    const [search, setSearch] =
        useState("");


    // ============================================================
    // Load permissions
    // ============================================================

    useEffect(() => {

        async function loadPermissions() {

            if (!roleId) {

                setError("Role ID is missing.");

                setLoading(false);

                return;
            }


            try {

                setLoading(true);

                setError("");

                setSuccess("");


                // Get ALL available permissions

                const allPermissions =
                    await getPermissions();


                // Get permissions currently assigned

                const assignedPermissions =
                    await getRolePermissions(
                        roleId
                    );


                // Create a set of assigned permission IDs

                const assignedIds =
                new Set(
                    assignedPermissions.map(
                        permission =>
                            permission.permissionId
                    )
                );


                // Remember original state

                setOriginalAssignedIds(
                    assignedIds
                );


                // Merge all permissions with assignment state

                const mergedPermissions =
                    allPermissions.map(
                        permission => {

                            const isAssigned =
                                assignedIds.has(
                                    permission.id
                                );


                            return {
                                ...permission,
                                isAssigned,
                            };

                        }
                    );


                setPermissions(
                    mergedPermissions
                );

            } catch (err) {

                console.error(
                    "Failed to load role permissions:",
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


        loadPermissions();

    }, [roleId]);


    // ============================================================
    // Filter permissions
    // ============================================================

    const filteredPermissions =
        useMemo(() => {

            const value =
                search
                    .trim()
                    .toLowerCase();


            if (!value) {

                return permissions;

            }


            return permissions.filter(
                permission =>
                    permission.name
                        .toLowerCase()
                        .includes(value)
                    ||
                    permission.code
                        .toLowerCase()
                        .includes(value)
            );

        }, [
            permissions,
            search,
        ]);


    // ============================================================
    // Statistics
    // ============================================================

    const totalPermissions =
        permissions.length;


    const assignedCount =
        permissions.filter(
            permission =>
                permission.isAssigned
        ).length;


    const availableCount =
        permissions.filter(
            permission =>
                !permission.isAssigned
        ).length;


    // ============================================================
    // Toggle permission
    // ============================================================

    function togglePermission(
        permissionId: string
    ) {

        setPermissions(
            current =>
                current.map(
                    permission =>
                        permission.id === permissionId
                            ? {
                                ...permission,
                                isAssigned:
                                    !permission.isAssigned,
                            }
                            : permission
                )
        );


        setSuccess("");

        setError("");

    }


    // ============================================================
    // Select all
    // ============================================================

    function selectAll() {

        setPermissions(
            current =>
                current.map(
                    permission => ({
                        ...permission,
                        isAssigned: true,
                    })
                )
        );


        setSuccess("");

        setError("");

    }


    // ============================================================
    // Clear all
    // ============================================================

    function clearAll() {

        setPermissions(
            current =>
                current.map(
                    permission => ({
                        ...permission,
                        isAssigned: false,
                    })
                )
        );


        setSuccess("");

        setError("");

    }


    // ============================================================
    // Save
    // ============================================================

    async function handleSave() {

        if (!roleId) {

            setError("Role ID is missing.");

            return;
        }


        try {

            setSaving(true);

            setError("");

            setSuccess("");


            // ====================================================
            // Current assigned permission IDs
            // ====================================================

            const currentAssignedIds =
                new Set(
                    permissions
                        .filter(
                            permission =>
                                permission.isAssigned
                        )
                        .map(
                            permission =>
                                permission.id
                        )
                );


            // ====================================================
            // Find permissions that were added
            // ====================================================

            const permissionsToAdd =
                permissions.filter(
                    permission =>
                        permission.isAssigned &&
                        !originalAssignedIds.has(
                            permission.id
                        )
                );


            // ====================================================
            // Find permissions that were removed
            // ====================================================

            const permissionsToRemove =
                permissions.filter(
                    permission =>
                        !permission.isAssigned &&
                        originalAssignedIds.has(
                            permission.id
                        )
                );


            // ====================================================
            // Assign new permissions
            // ====================================================

            for (
                const permission
                of permissionsToAdd
            ) {

                await assignRolePermission(
                    roleId,
                    permission.id
                );

            }


            // ====================================================
            // Remove permissions
            // ====================================================

            for (
                const permission
                of permissionsToRemove
            ) {

                await removeRolePermission(
                    roleId,
                    permission.id
                );

            }


            // ====================================================
            // Update original state
            // ====================================================

            setOriginalAssignedIds(
                currentAssignedIds
            );


            setSuccess(
                "Role permissions updated successfully."
            );

        } catch (err) {

            console.error(
                "Failed to update role permissions:",
                err
            );


            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to update role permissions."
            );

        } finally {

            setSaving(false);

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

                        Roles

                        <span>
                            /
                        </span>

                        Permissions

                    </div>


                    <h1>
                        Role Permissions
                    </h1>


                    <p>
                        Manage the permissions assigned
                        to this role.
                    </p>

                </div>


                <button
                    type="button"
                    className="primary-button"
                    onClick={() =>
                        navigate("/roles")
                    }
                >

                    <span className="button-plus">
                        ←
                    </span>

                    Back to Roles

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
                Success
            ==================================================== */}

            {success && (

                <div className="roles-success">

                    <span className="roles-success-icon">
                        ✓
                    </span>


                    <span>
                        {success}
                    </span>

                </div>

            )}


            {/* ====================================================
                Statistics
            ==================================================== */}

            <div className="role-statistics">


                {/* Total */}

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


                {/* Assigned */}

                <div className="role-stat-card">

                    <div className="role-stat-icon active">
                        ✓
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Assigned
                        </span>


                        <strong>
                            {assignedCount}
                        </strong>

                    </div>

                </div>


                {/* Available */}

                <div className="role-stat-card">

                    <div className="role-stat-icon inactive">
                        −
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Available
                        </span>


                        <strong>
                            {availableCount}
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
                            value={search}
                            onChange={event =>
                                setSearch(
                                    event.target.value
                                )
                            }
                        />

                    </div>


                    <div className="permission-toolbar-actions">

                        <button
                            type="button"
                            onClick={selectAll}
                            disabled={
                                permissions.length === 0 ||
                                saving
                            }
                        >
                            Select All
                        </button>


                        <button
                            type="button"
                            onClick={clearAll}
                            disabled={
                                permissions.length === 0 ||
                                saving
                            }
                        >
                            Clear All
                        </button>

                    </div>

                </div>


                {/* =================================================
                    Permission List
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
                                ? "No permissions are currently available."
                                : "Try adjusting your search."
                            }
                        </p>

                    </div>

                ) : (

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
                                                        🔐
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
                                                        permission.isAssigned
                                                            ? "role-status active"
                                                            : "role-status inactive"
                                                    }
                                                >

                                                    <span className="status-dot">
                                                    </span>


                                                    {permission.isAssigned
                                                        ? "Assigned"
                                                        : "Not Assigned"}

                                                </span>

                                            </td>


                                            {/* Actions */}

                                            <td>

                                                <div className="role-actions">

                                                    <button
                                                        type="button"
                                                        className={
                                                            permission.isAssigned
                                                                ? "table-action deactivate"
                                                                : "table-action permissions"
                                                        }
                                                        onClick={() =>
                                                            togglePermission(
                                                                permission.id
                                                            )
                                                        }
                                                        disabled={
                                                            saving ||
                                                            !permission.isActive
                                                        }
                                                    >

                                                        {permission.isAssigned
                                                            ? "Remove"
                                                            : "Assign"}

                                                    </button>

                                                </div>

                                            </td>

                                        </tr>

                                    )
                                )}

                            </tbody>

                        </table>

                    </div>

                )}


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


                    <div className="role-actions">


                        <button
                            type="button"
                            className="table-action edit"
                            onClick={() =>
                                navigate("/roles")
                            }
                            disabled={saving}
                        >
                            Cancel
                        </button>


                        <button
                            type="button"
                            className="table-action permissions"
                            onClick={handleSave}
                            disabled={
                                saving ||
                                permissions.length === 0
                            }
                        >

                            {saving
                                ? "Saving..."
                                : "Save Permissions"}

                        </button>

                    </div>

                </div>

            </div>

        </div>

    );

}