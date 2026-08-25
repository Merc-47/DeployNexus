import { useEffect, useMemo, useState } from "react";

import OrganizationForm from "./OrganizationForm";

import {
    createOrganization,
    deactivateOrganization,
    getOrganizations,
    updateOrganization,
} from "../../services/api/organizationsApi";

import type {
    Organization,
} from "../../types/organization";


export default function OrganizationsPage() {


    // ============================================================
    // State
    // ============================================================

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

    const [editingOrganization, setEditingOrganization] =
        useState<Organization | null>(null);

    const [searchTerm, setSearchTerm] =
        useState("");

    const [statusFilter, setStatusFilter] =
        useState("all");


    // ============================================================
    // Load organizations
    // ============================================================

    async function loadOrganizations() {

        try {

            setLoading(true);
            setError("");

            const data =
                await getOrganizations();

            setOrganizations(data);

        } catch (err) {

            console.error(
                "Failed to load organizations:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to load organizations."
            );

        } finally {

            setLoading(false);

        }
    }


    useEffect(() => {

        loadOrganizations();

    }, []);


    // ============================================================
    // Statistics
    // ============================================================

    const totalOrganizations =
        organizations.length;

    const activeOrganizations =
        organizations.filter(
            organization =>
                organization.isActive
        ).length;

    const inactiveOrganizations =
        organizations.filter(
            organization =>
                !organization.isActive
        ).length;


    // ============================================================
    // Filter organizations
    // ============================================================

    const filteredOrganizations =
        useMemo(() => {

            const search =
                searchTerm
                    .trim()
                    .toLowerCase();

            return organizations.filter(
                organization => {

                    const matchesSearch =
                        !search ||
                        organization.name
                            .toLowerCase()
                            .includes(search) ||
                        organization.code
                            .toLowerCase()
                            .includes(search);


                    const matchesStatus =
                        statusFilter === "all" ||
                        (
                            statusFilter === "active" &&
                            organization.isActive
                        ) ||
                        (
                            statusFilter === "inactive" &&
                            !organization.isActive
                        );


                    return (
                        matchesSearch &&
                        matchesStatus
                    );

                }
            );

        }, [
            organizations,
            searchTerm,
            statusFilter,
        ]);


    // ============================================================
    // Create
    // ============================================================

    function handleCreate() {

        setEditingOrganization(null);

        setShowForm(true);

    }


    // ============================================================
    // Edit
    // ============================================================

    function handleEdit(
        organization: Organization
    ) {

        setEditingOrganization(
            organization
        );

        setShowForm(true);

    }


    // ============================================================
    // Submit
    // ============================================================

    async function handleSubmit(
        name: string,
        code: string
    ) {

        try {

            setFormLoading(true);

            setError("");


            // ====================================================
            // Update
            // ====================================================

            if (editingOrganization) {

                const updatedOrganization =
                    await updateOrganization(
                        editingOrganization.id,
                        {
                            name,
                            code,
                        }
                    );


                setOrganizations(
                    current =>
                        current.map(
                            organization =>
                                organization.id ===
                                updatedOrganization.id
                                    ? updatedOrganization
                                    : organization
                        )
                );

            }


            // ====================================================
            // Create
            // ====================================================

            else {

                const newOrganization =
                    await createOrganization({
                        name,
                        code,
                    });


                setOrganizations(
                    current => [
                        ...current,
                        newOrganization,
                    ]
                );

            }


            // ====================================================
            // Close form
            // ====================================================

            setShowForm(false);

            setEditingOrganization(null);

        } catch (err) {

            console.error(
                "Failed to save organization:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to save organization."
            );

        } finally {

            setFormLoading(false);

        }

    }


    // ============================================================
    // Deactivate
    // ============================================================

    async function handleDeactivate(
        organization: Organization
    ) {

        const confirmed =
            window.confirm(
                `Are you sure you want to deactivate "${organization.name}"?`
            );


        if (!confirmed) {

            return;

        }


        try {

            setError("");


            await deactivateOrganization(
                organization.id
            );


            setOrganizations(
                current =>
                    current.map(
                        item =>
                            item.id ===
                            organization.id
                                ? {
                                    ...item,
                                    isActive: false,
                                }
                                : item
                    )
            );

        } catch (err) {

            console.error(
                "Failed to deactivate organization:",
                err
            );

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to deactivate organization."
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
                        Loading organizations...
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


                    {/* Breadcrumb */}

                    <div className="breadcrumb">

                        Administration

                        <span>
                            /
                        </span>

                        Organizations

                    </div>


                    {/* Title */}

                    <h1>
                        Organizations
                    </h1>


                    <p>
                        Manage organizations and their
                        system access.
                    </p>

                </div>


                {/* Create button */}

                <button
                    type="button"
                    className="primary-button"
                    onClick={handleCreate}
                >

                    <span className="button-plus">
                        +
                    </span>

                    Create Organization

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


                {/* Total */}

                <div className="role-stat-card">

                    <div className="role-stat-icon total">
                        🏢
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Total Organizations
                        </span>


                        <strong>
                            {totalOrganizations}
                        </strong>

                    </div>

                </div>


                {/* Active */}

                <div className="role-stat-card">

                    <div className="role-stat-icon active">
                        ✓
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Active
                        </span>


                        <strong>
                            {activeOrganizations}
                        </strong>

                    </div>

                </div>


                {/* Inactive */}

                <div className="role-stat-card">

                    <div className="role-stat-icon inactive">
                        −
                    </div>


                    <div className="role-stat-content">

                        <span>
                            Inactive
                        </span>


                        <strong>
                            {inactiveOrganizations}
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


                    {/* Search */}

                    <div className="roles-search">

                        <span>
                            🔍
                        </span>


                        <input
                            type="text"
                            placeholder="Search organizations..."
                            value={searchTerm}
                            onChange={event =>
                                setSearchTerm(
                                    event.target.value
                                )
                            }
                        />

                    </div>


                    {/* Status filter */}

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
                                All organizations
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

                {filteredOrganizations.length === 0 ? (

                    <div className="roles-empty">


                        <div className="roles-empty-icon">
                            🏢
                        </div>


                        <h2>
                            No organizations found
                        </h2>


                        <p>

                            {organizations.length === 0
                                ? "Create your first organization to get started."
                                : "Try adjusting your search or filters."
                            }

                        </p>


                        {organizations.length === 0 && (

                            <button
                                type="button"
                                className="primary-button"
                                onClick={handleCreate}
                            >

                                + Create Organization

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


                                {/* Header */}

                                <thead>

                                    <tr>

                                        <th>
                                            ORGANIZATION
                                        </th>


                                        <th>
                                            CODE
                                        </th>


                                        <th>
                                            STATUS
                                        </th>


                                        <th className="actions-header">
                                            ACTIONS
                                        </th>

                                    </tr>

                                </thead>


                                {/* Body */}

                                <tbody>

                                    {filteredOrganizations.map(
                                        organization => (

                                            <tr
                                                key={
                                                    organization.id
                                                }
                                            >


                                                {/* Organization */}

                                                <td>

                                                    <div className="role-name-cell">


                                                        <div className="role-avatar">

                                                            {organization.name
                                                                .charAt(0)
                                                                .toUpperCase()}

                                                        </div>


                                                        <div className="role-name-info">

                                                            <strong>
                                                                {organization.name}
                                                            </strong>


                                                            <span>
                                                                Organization
                                                            </span>

                                                        </div>

                                                    </div>

                                                </td>


                                                {/* Code */}

                                                <td>

                                                    <span className="role-description">

                                                        {organization.code}

                                                    </span>

                                                </td>


                                                {/* Status */}

                                                <td>

                                                    <span
                                                        className={
                                                            organization.isActive
                                                                ? "role-status active"
                                                                : "role-status inactive"
                                                        }
                                                    >

                                                        <span className="status-dot">
                                                        </span>


                                                        {organization.isActive
                                                            ? "Active"
                                                            : "Inactive"}

                                                    </span>

                                                </td>


                                                {/* Actions */}

                                                <td>

                                                    <div className="role-actions">


                                                        {/* Edit */}

                                                        <button
                                                            type="button"
                                                            className="table-action edit"
                                                            onClick={() =>
                                                                handleEdit(
                                                                    organization
                                                                )
                                                            }
                                                            disabled={
                                                                !organization.isActive
                                                            }
                                                        >

                                                            Edit

                                                        </button>


                                                        {/* Deactivate */}

                                                        <button
                                                            type="button"
                                                            className="table-action deactivate"
                                                            onClick={() =>
                                                                handleDeactivate(
                                                                    organization
                                                                )
                                                            }
                                                            disabled={
                                                                !organization.isActive
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
                                    {filteredOrganizations.length}
                                </strong>{" "}

                                of{" "}

                                <strong>
                                    {organizations.length}
                                </strong>{" "}

                                organizations

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

                        <OrganizationForm
                            organization={
                                editingOrganization
                            }
                            onSubmit={
                                handleSubmit
                            }
                            onCancel={() => {

                                setShowForm(false);

                                setEditingOrganization(
                                    null
                                );

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