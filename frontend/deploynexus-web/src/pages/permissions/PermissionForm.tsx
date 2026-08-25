import { useEffect, useState } from "react";

import type {
    Permission,
    CreatePermissionRequest,
    UpdatePermissionRequest,
} from "../../types/permissions";


interface PermissionFormProps {

    permission: Permission | null;

    onSubmit: (
        data:
            | CreatePermissionRequest
            | UpdatePermissionRequest
    ) => Promise<void>;

    onCancel: () => void;

    loading: boolean;
}


export default function PermissionForm({
    permission,
    onSubmit,
    onCancel,
    loading,
}: PermissionFormProps) {


    // ============================================================
    // State
    // ============================================================

    const [name, setName] =
        useState("");

    const [code, setCode] =
        useState("");

    const [moduleId, setModuleId] =
        useState("");


    const [error, setError] =
        useState("");


    // ============================================================
    // Load existing permission
    // ============================================================

    useEffect(() => {

        if (permission) {

            setName(
                permission.name
            );

            setCode(
                permission.code
            );

            setModuleId(
                permission.moduleId
            );

        } else {

            setName("");
            setCode("");
            setModuleId("");

        }

        setError("");

    }, [permission]);


    // ============================================================
    // Submit
    // ============================================================

    async function handleSubmit(
        event: React.FormEvent
    ) {

        event.preventDefault();

        setError("");


        const trimmedName =
            name.trim();

        const trimmedCode =
            code.trim();

        const trimmedModuleId =
            moduleId.trim();


        if (!trimmedName) {

            setError(
                "Permission name is required."
            );

            return;
        }


        if (!trimmedCode) {

            setError(
                "Permission code is required."
            );

            return;
        }


        if (!permission && !trimmedModuleId) {

            setError(
                "Module ID is required."
            );

            return;
        }


        try {

            if (permission) {

                await onSubmit({
                    name: trimmedName,
                    code: trimmedCode,
                });

            } else {

                await onSubmit({
                    name: trimmedName,
                    code: trimmedCode,
                    moduleId: trimmedModuleId,
                });
            }

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

        }

    }


    // ============================================================
    // Form
    // ============================================================

    return (

        <form
            className="role-form"
            onSubmit={handleSubmit}
        >

            {/* ====================================================
                Header
            ==================================================== */}

            <div className="role-form-header">

                <div>

                    <p className="role-form-eyebrow">
                        ACCESS CONTROL
                    </p>

                    <h2>
                        {permission
                            ? "Edit Permission"
                            : "Create Permission"}
                    </h2>

                    <p>
                        {permission
                            ? "Update the permission details."
                            : "Create a permission that can be assigned to roles."}
                    </p>

                </div>


                <button
                    type="button"
                    className="role-form-close"
                    onClick={onCancel}
                    disabled={loading}
                >
                    ×
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

                </div>

            )}


            {/* ====================================================
                Fields
            ==================================================== */}

            <div className="role-form-body">

                {/* Name */}

                <div className="form-group">

                    <label htmlFor="permission-name">
                        Permission Name
                    </label>

                    <input
                        id="permission-name"
                        type="text"
                        value={name}
                        onChange={event =>
                            setName(
                                event.target.value
                            )
                        }
                        placeholder="e.g. Manage Users"
                        disabled={loading}
                    />

                </div>


                {/* Code */}

                <div className="form-group">

                    <label htmlFor="permission-code">
                        Permission Code
                    </label>

                    <input
                        id="permission-code"
                        type="text"
                        value={code}
                        onChange={event =>
                            setCode(
                                event.target.value
                            )
                        }
                        placeholder="e.g. USER_MANAGE"
                        disabled={loading}
                    />

                    <span className="form-help-text">
                        Use a unique code for authorization checks.
                    </span>

                </div>


                {/* Module ID */}

                <div className="form-group">

                    <label htmlFor="permission-module">
                        Module ID
                    </label>

                    <input
                        id="permission-module"
                        type="text"
                        value={moduleId}
                        onChange={event =>
                            setModuleId(
                                event.target.value
                            )
                        }
                        placeholder="Module GUID"
                        disabled={
                            loading ||
                            permission !== null
                        }
                    />

                    {permission && (

                        <span className="form-help-text">
                            Module cannot be changed when editing a permission.
                        </span>

                    )}

                </div>

            </div>


            {/* ====================================================
                Footer
            ==================================================== */}

            <div className="role-form-footer">

                <button
                    type="button"
                    className="secondary-button"
                    onClick={onCancel}
                    disabled={loading}
                >
                    Cancel
                </button>


                <button
                    type="submit"
                    className="primary-button"
                    disabled={loading}
                >

                    {loading
                        ? "Saving..."
                        : permission
                            ? "Update Permission"
                            : "Create Permission"}

                </button>

            </div>

        </form>

    );
}