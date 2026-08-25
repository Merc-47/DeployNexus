import { useEffect, useState } from "react";

import type {
    Organization,
} from "../../types/organization";


interface OrganizationFormProps {

    organization: Organization | null;

    onSubmit: (
        name: string,
        code: string
    ) => Promise<void>;

    onCancel: () => void;

    loading: boolean;

}


export default function OrganizationForm({
    organization,
    onSubmit,
    onCancel,
    loading,
}: OrganizationFormProps) {


    // ============================================================
    // State
    // ============================================================

    const [name, setName] =
        useState("");

    const [code, setCode] =
        useState("");

    const [validationError, setValidationError] =
        useState("");


    // ============================================================
    // Load organization when editing
    // ============================================================

    useEffect(() => {

        if (organization) {

            setName(
                organization.name
            );

            setCode(
                organization.code
            );

        } else {

            setName("");
            setCode("");

        }

        setValidationError("");

    }, [organization]);


    // ============================================================
    // Submit
    // ============================================================

    async function handleSubmit(
        event: React.FormEvent
    ) {

        event.preventDefault();

        setValidationError("");


        const trimmedName =
            name.trim();

        const trimmedCode =
            code.trim();


        if (!trimmedName) {

            setValidationError(
                "Organization name is required."
            );

            return;
        }


        if (!trimmedCode) {

            setValidationError(
                "Organization code is required."
            );

            return;
        }


        await onSubmit(
            trimmedName,
            trimmedCode
        );

    }


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
                        ADMINISTRATION
                    </p>

                    <h2>
                        {organization
                            ? "Edit Organization"
                            : "Create Organization"}
                    </h2>

                    <p>
                        {organization
                            ? "Update the organization details."
                            : "Create a new organization for your system."}
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
                Validation error
            ==================================================== */}

            {validationError && (

                <div className="roles-error">

                    <span className="roles-error-icon">
                        !
                    </span>

                    <span>
                        {validationError}
                    </span>

                </div>

            )}


            {/* ====================================================
                Form body
            ==================================================== */}

            <div className="role-form-body">


                {/* Name */}

                <div className="form-group">

                    <label htmlFor="organization-name">
                        Organization Name
                    </label>

                    <input
                        id="organization-name"
                        type="text"
                        placeholder="Enter organization name"
                        value={name}
                        onChange={event =>
                            setName(
                                event.target.value
                            )
                        }
                        disabled={loading}
                        maxLength={100}
                    />

                </div>


                {/* Code */}

                <div className="form-group">

                    <label htmlFor="organization-code">
                        Organization Code
                    </label>

                    <input
                        id="organization-code"
                        type="text"
                        placeholder="Enter organization code"
                        value={code}
                        onChange={event =>
                            setCode(
                                event.target.value
                            )
                        }
                        disabled={loading}
                        maxLength={50}
                    />

                    <span className="form-help">
                        Use a unique code to identify this organization.
                    </span>

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
                        : organization
                            ? "Update Organization"
                            : "Create Organization"}

                </button>

            </div>


        </form>

    );

}