import { useEffect, useState } from "react";

import type {
    User,
} from "../../types/user";

import type {
    Organization,
} from "../../types/organization";


interface UserFormProps {

    user: User | null;

    organizations: Organization[];

    onSubmit: (
        username: string,
        email: string,
        password: string,
        firstName: string,
        lastName: string,
        organizationId: string
    ) => Promise<void>;

    onCancel: () => void;

    loading: boolean;

}


export default function UserForm({
    user,
    organizations,
    onSubmit,
    onCancel,
    loading,
}: UserFormProps) {


    // ============================================================
    // State
    // ============================================================

    const [username, setUsername] =
        useState("");

    const [email, setEmail] =
        useState("");

    const [password, setPassword] =
        useState("");

    const [firstName, setFirstName] =
        useState("");

    const [lastName, setLastName] =
        useState("");

    const [organizationId, setOrganizationId] =
        useState("");

    const [validationError, setValidationError] =
        useState("");


    // ============================================================
    // Load user when editing
    // ============================================================

    useEffect(() => {

        if (user) {

            setUsername(
                user.username
            );

            setEmail(
                user.email
            );

            setFirstName(
                user.firstName
            );

            setLastName(
                user.lastName
            );

            setOrganizationId(
                user.organizationId
            );

            setPassword("");

        } else {

            setUsername("");
            setEmail("");
            setPassword("");
            setFirstName("");
            setLastName("");
            setOrganizationId("");

        }

        setValidationError("");

    }, [user]);


    // ============================================================
    // Submit
    // ============================================================

    async function handleSubmit(
        event: React.FormEvent
    ) {

        event.preventDefault();

        setValidationError("");


        const trimmedUsername =
            username.trim();

        const trimmedEmail =
            email.trim();

        const trimmedFirstName =
            firstName.trim();

        const trimmedLastName =
            lastName.trim();


        if (!trimmedFirstName) {

            setValidationError(
                "First name is required."
            );

            return;

        }


        if (!trimmedLastName) {

            setValidationError(
                "Last name is required."
            );

            return;

        }


        if (!trimmedUsername) {

            setValidationError(
                "Username is required."
            );

            return;

        }


        if (!trimmedEmail) {

            setValidationError(
                "Email is required."
            );

            return;

        }


        if (!organizationId) {

            setValidationError(
                "Organization is required."
            );

            return;

        }


        if (!user && !password.trim()) {

            setValidationError(
                "Password is required."
            );

            return;

        }


        await onSubmit(
            trimmedUsername,
            trimmedEmail,
            password,
            trimmedFirstName,
            trimmedLastName,
            organizationId
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

                        {user
                            ? "Edit User"
                            : "Create User"}

                    </h2>


                    <p>

                        {user
                            ? "Update the user's account details."
                            : "Create a new user account for your system."}

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


                {/* First Name */}

                <div className="form-group">

                    <label htmlFor="user-first-name">
                        First Name
                    </label>


                    <input
                        id="user-first-name"
                        type="text"
                        placeholder="Enter first name"
                        value={firstName}
                        onChange={event =>
                            setFirstName(
                                event.target.value
                            )
                        }
                        disabled={loading}
                        maxLength={100}
                    />

                </div>


                {/* Last Name */}

                <div className="form-group">

                    <label htmlFor="user-last-name">
                        Last Name
                    </label>


                    <input
                        id="user-last-name"
                        type="text"
                        placeholder="Enter last name"
                        value={lastName}
                        onChange={event =>
                            setLastName(
                                event.target.value
                            )
                        }
                        disabled={loading}
                        maxLength={100}
                    />

                </div>


                {/* Username */}

                <div className="form-group">

                    <label htmlFor="user-username">
                        Username
                    </label>


                    <input
                        id="user-username"
                        type="text"
                        placeholder="Enter username"
                        value={username}
                        onChange={event =>
                            setUsername(
                                event.target.value
                            )
                        }
                        disabled={loading}
                        maxLength={100}
                    />

                </div>


                {/* Email */}

                <div className="form-group">

                    <label htmlFor="user-email">
                        Email Address
                    </label>


                    <input
                        id="user-email"
                        type="email"
                        placeholder="Enter email address"
                        value={email}
                        onChange={event =>
                            setEmail(
                                event.target.value
                            )
                        }
                        disabled={loading}
                        maxLength={255}
                    />

                </div>


                {/* Password */}

                {!user && (

                    <div className="form-group">

                        <label htmlFor="user-password">
                            Password
                        </label>


                        <input
                            id="user-password"
                            type="password"
                            placeholder="Enter password"
                            value={password}
                            onChange={event =>
                                setPassword(
                                    event.target.value
                                )
                            }
                            disabled={loading}
                        />


                        <span className="form-help">
                            A password is required when creating a new user.
                        </span>

                    </div>

                )}


                {/* Organization */}

                <div className="form-group">

                    <label htmlFor="user-organization">
                        Organization
                    </label>


                    <select
                        id="user-organization"
                        value={organizationId}
                        onChange={event =>
                            setOrganizationId(
                                event.target.value
                            )
                        }
                        disabled={loading}
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
                                        {organization.name}
                                    </option>

                                )
                            )}

                    </select>


                    <span className="form-help">
                        Only active organizations are available.
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
                        : user
                            ? "Update User"
                            : "Create User"}

                </button>

            </div>

        </form>

    );

}