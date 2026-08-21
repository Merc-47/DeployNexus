import { useEffect, useState } from "react";
import type { FormEvent } from "react";

import type {
    Role,
    Organization,
} from "../../types/roles";


interface RoleFormProps {

    role?: Role | null;

    organizations: Organization[];

    onSubmit: (
        name: string,
        description: string,
        organizationId: string
    ) => Promise<void>;

    onCancel: () => void;

    loading?: boolean;
}


export default function RoleForm({
    role,
    organizations,
    onSubmit,
    onCancel,
    loading = false,
}: RoleFormProps) {

    const [name, setName] =
        useState("");

    const [description, setDescription] =
        useState("");

    const [organizationId, setOrganizationId] =
        useState("");

    const [error, setError] =
        useState("");

    const isEditing =
        !!role;


    useEffect(() => {

        if (role) {

            setName(
                role.name
            );

            setDescription(
                role.description || ""
            );

            setOrganizationId(
                role.organizationId
            );

        } else {

            setName("");
            setDescription("");
            setOrganizationId("");

        }

        setError("");

    }, [role]);


    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {

        event.preventDefault();

        setError("");

        if (!name.trim()) {

            setError(
                "Role name is required."
            );

            return;
        }

        if (!organizationId) {

            setError(
                "Please select an organization."
            );

            return;
        }

        try {

            await onSubmit(
                name.trim(),
                description.trim(),
                organizationId
            );

        } catch (err) {

            setError(
                err instanceof Error
                    ? err.message
                    : "Unable to save role."
            );
        }
    }


    return (

        <div className="role-form">

            {/* Header */}

            <div className="role-form-header">

                <div>

                    <span className="role-form-eyebrow">
                        ROLE MANAGEMENT
                    </span>

                    <h2>
                        {isEditing
                            ? "Edit Role"
                            : "Create Role"}
                    </h2>

                    <p>
                        {isEditing
                            ? "Update the role details below."
                            : "Create a role and assign it to an organization."}
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


            {/* Error */}

            {error && (

                <div className="role-form-error">

                    <span>
                        !
                    </span>

                    {error}

                </div>

            )}


            <form
                onSubmit={
                    handleSubmit
                }
            >

                {/* Name */}

                <div className="role-form-group">

                    <label htmlFor="role-name">
                        Role Name
                    </label>

                    <input
                        id="role-name"
                        type="text"
                        value={name}
                        onChange={event =>
                            setName(
                                event.target.value
                            )
                        }
                        placeholder="e.g. Administrator"
                        disabled={loading}
                    />

                </div>


                {/* Description */}

                <div className="role-form-group">

                    <label htmlFor="role-description">
                        Description
                    </label>

                    <textarea
                        id="role-description"
                        value={description}
                        onChange={event =>
                            setDescription(
                                event.target.value
                            )
                        }
                        placeholder="Describe what this role is used for"
                        rows={4}
                        disabled={loading}
                    />

                </div>


                {/* Organization */}

                <div className="role-form-group">

                    <label htmlFor="role-organization">
                        Organization
                    </label>

                    <select
                        id="role-organization"
                        value={organizationId}
                        onChange={event =>
                            setOrganizationId(
                                event.target.value
                            )
                        }
                        disabled={
                            loading ||
                            isEditing
                        }
                    >

                        <option value="">
                            Select organization
                        </option>

                        {organizations
                            .filter(
                                organization =>
                                    organization.isActive
                            )
                            .map(
                                organization => (

                                    <option
                                        key={
                                            organization.id
                                        }
                                        value={
                                            organization.id
                                        }
                                    >
                                        {
                                            organization.name
                                        }
                                        {" "}
                                        (
                                        {
                                            organization.code
                                        }
                                        )
                                    </option>

                                )
                            )}

                    </select>


                    {isEditing && (

                        <small className="role-form-help">
                            Organization cannot be changed
                            while editing a role.
                        </small>

                    )}

                </div>


                {/* Buttons */}

                <div className="role-form-actions">

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

                        {loading ? (
                            <>
                                <span className="button-spinner"></span>
                                Saving...
                            </>
                        ) : (
                            isEditing
                                ? "Update Role"
                                : "Create Role"
                        )}

                    </button>

                </div>

            </form>

        </div>
    );
}