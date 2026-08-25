const API_URL = import.meta.env.VITE_API_URL;

if (!API_URL) {
    throw new Error("VITE_API_URL is not configured.");
}

export async function apiClient<T>(
    endpoint: string,
    options: RequestInit = {}
): Promise<T> {

    const token =
        localStorage.getItem("deploynexus_token");

    const headers = new Headers(
        options.headers
    );

    headers.set(
        "Content-Type",
        "application/json"
    );

    if (token) {
        headers.set(
            "Authorization",
            `Bearer ${token}`
        );
    }

    const response = await fetch(
        `${API_URL}${endpoint}`,
        {
            ...options,
            headers,
        }
    );

    if (!response.ok) {

        let message =
            "An unexpected error occurred.";

        try {
            const errorData =
                await response.json();

            message =
                errorData.detail ||
                errorData.message ||
                message;
        } catch {
            // Response wasn't JSON.
        }

        throw new Error(message);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json();
}