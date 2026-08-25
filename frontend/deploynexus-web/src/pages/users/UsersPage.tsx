import { useEffect, useMemo, useState } from "react";

import UserForm from "./UserForm";

import {
    createUser,
    deactivateUser,
    getUsers,
    updateUser,
    assignUserRole,
} from "../../services/api/usersApi";

import {
    getOrganizations,
    getRoles,
} from "../../services/api/rolesApi";

import type {
    User,
} from "../../types/user";

import type {
    Organization,
    Role,
} from "../../types/roles";


export default function UsersPage() {


    // ============================================================
    // State
    // ============================================================

    const [users, setUsers] =
        useState<User[]>([]);

    const [organizations, setOrganizations] =
        useState<Organization[]>([]);

    const [roles, setRoles] =
        useState<Role[]>([]);

    const [loading, setLoading] =
        useState(true);

    const [formLoading, setFormLoading] =
        useState(false);

    const [roleLoading, setRoleLoading] =
        useState(false);

    const [error, setError] =
        useState("");

    const [showForm, setShowForm] =
        useState(false);

    const [editingUser, setEditingUser] =
        useState<User | null>(null);

    const [roleUser, setRoleUser] =
        useState<User | null>(null);

    const [selectedRoleId, setSelectedRoleId] =
        useState("");

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
                usersData,
                organizationsData,
                rolesData,
            ] = await Promise.all([
                getUsers(),
                getOrganizations(),
                getRoles(),
            ]);

            setUsers(usersData);
            setOrganizations(organizationsData);
            setRoles(rolesData);

        } catch (err) {

            console.error(
                "Failed to load users:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to load users."
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

    const totalUsers =
        users.length;

    const activeUsers =
        users.filter(
            user => user.isActive
        ).length;

    const inactiveUsers =
        users.filter(
            user => !user.isActive
        ).length;


    // ============================================================
    // Filter users
    // ============================================================

    const filteredUsers =
        useMemo(() => {

            const search =
                searchTerm
                    .trim()
                    .toLowerCase();

            return users.filter(
                user => {

                    const fullName =
                        `${user.firstName} ${user.lastName}`
                            .toLowerCase();

                    const matchesSearch =
                        !search ||
                        user.username
                            .toLowerCase()
                            .includes(search) ||
                        user.email
                            .toLowerCase()
                            .includes(search) ||
                        fullName.includes(search);


                    const matchesStatus =
                        statusFilter === "all" ||
                        (
                            statusFilter === "active" &&
                            user.isActive
                        ) ||
                        (
                            statusFilter === "inactive" &&
                            !user.isActive
                        );


                    return (
                        matchesSearch &&
                        matchesStatus
                    );

                }
            );

        }, [
            users,
            searchTerm,
            statusFilter,
        ]);


    // ============================================================
    // Create
    // ============================================================

    function handleCreate() {

        setEditingUser(null);
        setShowForm(true);
        setError("");

    }


    // ============================================================
    // Edit
    // ============================================================

    function handleEdit(
        user: User
    ) {

        setEditingUser(user);
        setShowForm(true);
        setError("");

    }


    // ============================================================
    // Close form
    // ============================================================

    function handleCloseForm() {

        setShowForm(false);
        setEditingUser(null);

    }


    // ============================================================
    // Submit user
    // ============================================================

    async function handleSubmit(
        username: string,
        email: string,
        password: string,
        firstName: string,
        lastName: string,
        organizationId: string
    ) {

        try {

            setFormLoading(true);
            setError("");


            // ====================================================
            // Update
            // ====================================================

            if (editingUser) {

                const updatedUser =
                    await updateUser(
                        editingUser.id,
                        {
                            username,
                            email,
                            firstName,
                            lastName,
                            isActive:
                                editingUser.isActive,
                            organizationId,
                        }
                    );


                setUsers(
                    current =>
                        current.map(
                            user =>
                                user.id ===
                                updatedUser.id
                                    ? updatedUser
                                    : user
                        )
                );

            }


            // ====================================================
            // Create
            // ====================================================

            else {

                const newUser =
                    await createUser({
                        username,
                        email,
                        password,
                        firstName,
                        lastName,
                        organizationId,
                    });


                setUsers(
                    current => [
                        ...current,
                        newUser,
                    ]
                );

            }


            handleCloseForm();

        } catch (err) {

            console.error(
                "Failed to save user:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to save user."
            );

        } finally {

            setFormLoading(false);

        }
    }


    // ============================================================
    // Manage role
    // ============================================================

    function handleManageRole(
        user: User
    ) {

        setRoleUser(user);

        setSelectedRoleId(
            user.roleId ?? ""
        );

        setError("");

    }


    // ============================================================
    // Save role
    // ============================================================

    async function handleSaveRole() {

        if (!roleUser) {

            return;

        }


        try {

            setRoleLoading(true);
            setError("");


            const updatedUser =
                await assignUserRole(
                    roleUser.id,
                    {
                        roleId:
                            selectedRoleId || null,
                    }
                );


            setUsers(
                current =>
                    current.map(
                        user =>
                            user.id ===
                            updatedUser.id
                                ? updatedUser
                                : user
                    )
            );


            setRoleUser(null);
            setSelectedRoleId("");

        } catch (err) {

            console.error(
                "Failed to assign user role:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to assign user role."
            );

        } finally {

            setRoleLoading(false);

        }
    }


    // ============================================================
    // Deactivate
    // ============================================================

    async function handleDeactivate(
        user: User
    ) {

        const confirmed =
            window.confirm(
                `Are you sure you want to deactivate "${user.username}"?`
            );


        if (!confirmed) {

            return;

        }


        try {

            setError("");


            await deactivateUser(
                user.id
            );


            setUsers(
                current =>
                    current.map(
                        item =>
                            item.id === user.id
                                ? {
                                    ...item,
                                    isActive: false,
                                }
                                : item
                    )
            );

        } catch (err) {

            console.error(
                "Failed to deactivate user:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to deactivate user."
            );

        }
    }


    // ============================================================
    // Helpers
    // ============================================================

    function getOrganizationName(
        organizationId: string
    ) {

        const organization =
            organizations.find(
                item =>
                    item.id === organizationId
            );


        return organization
            ? organization.name
            : "Unknown organization";
    }


    function getRoleName(
        roleId: string | null
    ) {

        if (!roleId) {

            return "No role assigned";

        }


        const role =
            roles.find(
                item =>
                    item.id === roleId
            );


        return role
            ? role.name
            : "Unknown role";
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
                        Loading users...
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

                        Users

                    </div>


                    <h1>
                        Users
                    </h1>


                    <p>
                        Manage users and control
                        system access.
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

                    Create User

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
                        👥
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Total Users
                        </span>


                        <strong>
                            {totalUsers}
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
                            {activeUsers}
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
                            {inactiveUsers}
                        </strong>

                    </div>

                </div>

            </div>


            {/* ====================================================
                Content
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
                            placeholder="Search users..."
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
                                All users
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
                    Empty
                ================================================= */}

                {filteredUsers.length === 0 ? (

                    <div className="roles-empty">

                        <div className="roles-empty-icon">
                            👥
                        </div>


                        <h2>
                            No users found
                        </h2>


                        <p>

                            {users.length === 0
                                ? "Create your first user to get started."
                                : "Try adjusting your search or filters."
                            }

                        </p>


                        {users.length === 0 && (

                            <button
                                type="button"
                                className="primary-button"
                                onClick={handleCreate}
                            >
                                + Create User
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
                                            USER
                                        </th>

                                        <th>
                                            EMAIL
                                        </th>

                                        <th>
                                            ORGANIZATION
                                        </th>

                                        <th>
                                            ROLE
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

                                    {filteredUsers.map(
                                        user => (

                                            <tr
                                                key={
                                                    user.id
                                                }
                                            >

                                                {/* User */}

                                                <td>

                                                    <div className="role-name-cell">

                                                        <div className="role-avatar">

                                                            {user.firstName
                                                                ? user.firstName
                                                                    .charAt(0)
                                                                    .toUpperCase()
                                                                : user.username
                                                                    .charAt(0)
                                                                    .toUpperCase()}

                                                        </div>


                                                        <div className="role-name-info">

                                                            <strong>

                                                                {user.firstName}{" "}
                                                                {user.lastName}

                                                            </strong>


                                                            <span>
                                                                @{user.username}
                                                            </span>

                                                        </div>

                                                    </div>

                                                </td>


                                                {/* Email */}

                                                <td>

                                                    <span className="role-description">

                                                        {user.email}

                                                    </span>

                                                </td>


                                                {/* Organization */}

                                                <td>

                                                    <span className="role-description">

                                                        {getOrganizationName(
                                                            user.organizationId
                                                        )}

                                                    </span>

                                                </td>


                                                {/* Role */}

                                                <td>

                                                    <span className="role-description">

                                                        {getRoleName(
                                                            user.roleId
                                                        )}

                                                    </span>

                                                </td>


                                                {/* Status */}

                                                <td>

                                                    <span
                                                        className={
                                                            user.isActive
                                                                ? "role-status active"
                                                                : "role-status inactive"
                                                        }
                                                    >

                                                        <span className="status-dot">
                                                        </span>


                                                        {user.isActive
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
                                                                handleManageRole(
                                                                    user
                                                                )
                                                            }
                                                            disabled={
                                                                !user.isActive
                                                            }
                                                        >
                                                            Manage Role
                                                        </button>


                                                        <button
                                                            type="button"
                                                            className="table-action edit"
                                                            onClick={() =>
                                                                handleEdit(
                                                                    user
                                                                )
                                                            }
                                                            disabled={
                                                                !user.isActive
                                                            }
                                                        >
                                                            Edit
                                                        </button>


                                                        <button
                                                            type="button"
                                                            className="table-action deactivate"
                                                            onClick={() =>
                                                                handleDeactivate(
                                                                    user
                                                                )
                                                            }
                                                            disabled={
                                                                !user.isActive
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


                        {/* Footer */}

                        <div className="roles-table-footer">

                            <span>

                                Showing{" "}

                                <strong>
                                    {filteredUsers.length}
                                </strong>{" "}

                                of{" "}

                                <strong>
                                    {users.length}
                                </strong>{" "}

                                users

                            </span>

                        </div>

                    </>

                )}

            </div>


            {/* ====================================================
                User Form Modal
            ==================================================== */}

            {showForm && (

                <div className="role-modal-overlay">

                    <div className="role-modal">

                        <UserForm
                            user={
                                editingUser
                            }
                            organizations={
                                organizations
                            }
                            onSubmit={
                                handleSubmit
                            }
                            onCancel={
                                handleCloseForm
                            }
                            loading={
                                formLoading
                            }
                        />

                    </div>

                </div>

            )}


            {/* ====================================================
                Role Modal
            ==================================================== */}

            {roleUser && (

                <div className="role-modal-overlay">

                    <div className="role-modal">

                        <div className="role-form-header">

                            <div>

                                <p className="role-form-eyebrow">
                                    ACCESS CONTROL
                                </p>

                                <h2>
                                    Manage Role
                                </h2>

                                <p>
                                    Assign a role to{" "}
                                    <strong>
                                        {roleUser.firstName}{" "}
                                        {roleUser.lastName}
                                    </strong>.
                                </p>

                            </div>


                            <button
                                type="button"
                                className="role-form-close"
                                onClick={() => {

                                    setRoleUser(null);
                                    setSelectedRoleId("");

                                }}
                                disabled={roleLoading}
                            >
                                ×
                            </button>

                        </div>


                        <div className="role-form-body">

                            <div className="form-group">

                                <label htmlFor="user-role">
                                    Role
                                </label>


                                <select
                                    id="user-role"
                                    value={
                                        selectedRoleId
                                    }
                                    onChange={event =>
                                        setSelectedRoleId(
                                            event.target.value
                                        )
                                    }
                                    disabled={
                                        roleLoading
                                    }
                                >

                                    <option value="">
                                        No role assigned
                                    </option>


                                    {roles
                                        .filter(
                                            role =>
                                                role.isActive &&
                                                role.organizationId ===
                                                    roleUser.organizationId
                                        )
                                        .map(
                                            role => (

                                                <option
                                                    key={
                                                        role.id
                                                    }
                                                    value={
                                                        role.id
                                                    }
                                                >
                                                    {role.name}
                                                </option>

                                            )
                                        )}

                                </select>


                                <span className="form-help">
                                    Only active roles belonging to the
                                    user's organization are available.
                                </span>

                            </div>

                        </div>


                        <div className="role-form-footer">

                            <button
                                type="button"
                                className="secondary-button"
                                onClick={() => {

                                    setRoleUser(null);
                                    setSelectedRoleId("");

                                }}
                                disabled={
                                    roleLoading
                                }
                            >
                                Cancel
                            </button>


                            <button
                                type="button"
                                className="primary-button"
                                onClick={
                                    handleSaveRole
                                }
                                disabled={
                                    roleLoading
                                }
                            >
                                {roleLoading
                                    ? "Saving..."
                                    : "Save Role"}
                            </button>

                        </div>

                    </div>

                </div>

            )}

        </div>

    );

}