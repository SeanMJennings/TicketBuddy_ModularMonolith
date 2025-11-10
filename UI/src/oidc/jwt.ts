export type JwtClaims = Record<string, unknown>;

const base64UrlToBase64 = (input: string): string => {
    const pad = input.length % 4 === 0 ? "" : "=".repeat(4 - (input.length % 4));
    return input.replace(/-/g, "+").replace(/_/g, "/") + pad;
};

const base64Decode = (b64: string): string => {
    if (typeof window !== "undefined" && typeof window.atob === "function") {
        return window.atob(b64);
    }

    const nodeBuffer = (globalThis as unknown as { Buffer?: typeof Buffer }).Buffer;
    if (nodeBuffer) {
        return nodeBuffer.from(b64, "base64").toString("utf-8");
    }

    throw new Error("No base64 decoder available in this environment");
};

export const decodeJwtClaims = (jwt: string | undefined): JwtClaims | null => {
    if (!jwt) return null;

    const parts = jwt.split(".");
    if (parts.length < 2) return null;

    try {
        const payloadB64url = parts[1];
        const payloadB64 = base64UrlToBase64(payloadB64url);
        const json = base64Decode(payloadB64);
        return JSON.parse(json) as JwtClaims;
    } catch {
        return null;
    }
};

export const extractScopes = (claims: JwtClaims | null): string[] => {
    if (!claims) return [];

    const scopes: string[] = [];

    const scopeVal = claims["scope"];
    if (typeof scopeVal === "string") {
        scopes.push(...scopeVal.split(/\s+/).filter(Boolean));
    }

    const scopesVal = claims["scopes"];
    if (Array.isArray(scopesVal)) {
        scopesVal.forEach((s) => {
            if (typeof s === "string") scopes.push(s);
        });
    }

    const realmAccess = claims["realm_access"];
    if (realmAccess && typeof realmAccess === "object") {
        const ra = realmAccess as Record<string, unknown>;
        if (Array.isArray(ra["roles"])) {
            (ra["roles"] as unknown[]).forEach((r) => {
                if (typeof r === "string") scopes.push(r);
            });
        }
    }

    const resourceAccess = claims["resource_access"];
    if (resourceAccess && typeof resourceAccess === "object") {
        const ra = resourceAccess as Record<string, unknown>;
        Object.values(ra).forEach((val) => {
            if (val && typeof val === "object") {
                const v = val as Record<string, unknown>;
                if (Array.isArray(v["roles"])) {
                    (v["roles"] as unknown[]).forEach((r) => {
                        if (typeof r === "string") scopes.push(r);
                    });
                }
            }
        });
    }

    return Array.from(new Set(scopes));
};