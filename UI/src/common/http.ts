﻿const api = import.meta.env.VITE_API_URL || '';

function redirectToErrorPage() {
    window.location.assign('/error');
}

export async function get<T>(url: string): Promise<T> {
    return fetch(api + url, {})
        .then(async response => {
            if (!response.ok) {
                if (response.status >= 500 && response.status < 600) {
                    redirectToErrorPage();
                }
                throw {
                    error: (await response.json()).error,
                    code: response.status
                };
            }
            return (await response.json());
        })
        .catch(handleNetworkError)
}

function handleNetworkError(error : { code: number | undefined }) {
    if (error?.code === undefined) {
        redirectToErrorPage();
        return;
    }
    throw error;
}

export async function post(url: string, body: unknown): Promise<unknown> {
    return fetch(api + url, {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(body)
    })
    .then(handleResponse())
    .catch(handleNetworkError);
}

export async function put(url: string, body: unknown): Promise<unknown> {
    return fetch(api + url, {
        method: "PUT",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(body)
    })
    .then(handleResponse())
    .catch(handleNetworkError);
}

export async function deleteCall(url: string): Promise<unknown> {
    return fetch(api + url, {
        method: "DELETE",
    })
    .then(handleResponse())
    .catch(handleNetworkError);
}

function handleResponse(): ((value: Response) => unknown) | null | undefined {
    return async (response) => {
        if (response.status === 204) return;
        if (!response.ok) {
            if (response.status >= 500) {
                redirectToErrorPage();
            }
            throw {
                errors: (await response?.json())?.Errors,
                code: response.status
            };
        }
        return (await response.json());
    };
}