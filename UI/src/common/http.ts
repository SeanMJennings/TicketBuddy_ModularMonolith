const api = import.meta.env.VITE_API_URL || '';

function redirectToErrorPage() {
    window.location.assign('/error');
}

export async function get<T>(url: string, jwt: string | null | undefined = undefined): Promise<T> {
    return fetch(api + url, {
        method: "GET",
        headers: getHeaders(jwt)
    })
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

function getHeaders(jwt: string | null | undefined) {
    const headers: Record<string, string> = {
        "Content-Type": "application/json"
    };
    if (jwt) {
        headers["Authorization"] = `Bearer ${jwt}`;
    }
    return headers;
}

function handleNetworkError(error : { code: number | undefined }) {
    if (error?.code === undefined) {
        redirectToErrorPage();
        return;
    }
    throw error;
}

export async function post(url: string, body: unknown, jwt: string | null | undefined = undefined): Promise<unknown> {
    return fetch(api + url, {
        method: "POST",
        headers: getHeaders(jwt),
        body: JSON.stringify(body),
    })
    .then(handleResponse())
    .catch(handleNetworkError);
}

export async function put(url: string, body: unknown, jwt: string | null | undefined = undefined): Promise<unknown> {
    return fetch(api + url, {
        method: "PUT",
        headers: getHeaders(jwt),
        body: JSON.stringify(body)
    })
    .then(handleResponse())
    .catch(handleNetworkError);
}

export async function deleteCall(url: string, jwt: string | null | undefined = undefined): Promise<unknown> {
    return fetch(api + url, {
        method: "DELETE",
        headers: getHeaders(jwt)
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