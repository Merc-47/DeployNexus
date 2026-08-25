const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

async function request<T>(
    endpoint: string,
    options: RequestInit = {}
): Promise<T> {

    const token = localStorage.getItem("token");

    const response = await fetch(
        `${API_BASE_URL}${endpoint}`,
        {
            ...options,

            headers: {
                "Content-Type": "application/json",

                ...(token
                    ? {
                        Authorization: `Bearer ${token}`
                    }
                    : {}),

                ...(options.headers || {})
            }
        }
    );

    if (!response.ok) {

        let message = "An error occurred";

        try {
            const error = await response.json();

            message =
                error.detail ||
                error.message ||
                message;

        } catch {
            // Response did not contain JSON.
        }

        throw new Error(message);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json();
}

export const apiClient = {

    get<T>(endpoint: string) {
        return request<T>(
            endpoint,
            {
                method: "GET"
            }
        );
    },

    post<T>(
        endpoint: string,
        data?: unknown
    ) {
        return request<T>(
            endpoint,
            {
                method: "POST",
                body: data
                    ? JSON.stringify(data)
                    : undefined
            }
        );
    },

    put<T>(
        endpoint: string,
        data?: unknown
    ) {
        return request<T>(
            endpoint,
            {
                method: "PUT",
                body: data
                    ? JSON.stringify(data)
                    : undefined
            }
        );
    },

    delete<T>(endpoint: string) {
        return request<T>(
            endpoint,
            {
                method: "DELETE"
            }
        );
    }
};