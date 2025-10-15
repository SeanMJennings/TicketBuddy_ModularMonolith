import {toast} from "react-toastify";

const api = import.meta.env.VITE_API_URL || '';

export async function get<T>(url: string): Promise<T> {
    return fetch(api + url, {})
        .then(async response => {
            if (!response.ok) {
                throw {
                    error: (await response.json()).error,
                    code: response.status
                };
            }
            return (await response.json());
        });
}

export async function post(url: string, body: unknown): Promise<unknown> {
    return fetch(api + url, {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(body)
    }).then(handleResponse())
}

export async function put(url: string, body: unknown): Promise<unknown> {
    return fetch(api + url, {
        method: "PUT",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(body)
    }).then(handleResponse())
}

export async function deleteCall(url: string): Promise<unknown> {
    return fetch(api + url, {
        method: "DELETE",
    })
        .then(handleResponse())
        .catch((err: Error) => {
            throw {
                message: err?.message || 'Network error',
                status: 0
            }
        })
}

export function handleError(error: ApiResponseError) {
    if (error.errors && Array.isArray(error.errors)) {
        error.errors.forEach((errorMessage: string) => {
            toast.error(errorMessage);
        });
    } else {
        toast.error('Failed to complete purchase. Please try again.');
    }
}

export type ApiResponseError = {
    errors: string[];
    code: number;
}

function handleResponse(): ((value: Response) => unknown) | null | undefined {
    return async (response) => {
        if (response.status === 204) return;
        if (!response.ok) {
            throw {
                errors: (await response?.json())?.Errors,
                code: response.status
            };
        }
        return (await response.json());
    };
}