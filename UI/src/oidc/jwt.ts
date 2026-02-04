export type JwtClaims = Record<string, unknown>;

const base64UrlToBase64 = (input: string): string => {
    const pad = input.length % 4 === 0 ? "" : "=".repeat(4 - (input.length % 4));
    return input.replace(/-/g, "+").replace(/_/g, "/") + pad;
};

export const decodeJwtClaims = (jwt: string | undefined): JwtClaims | null => {
    if (!jwt) return null;

    const parts = jwt.split(".");
    if (parts.length < 2) return null;

    try {
        const payloadB64url = parts[1];
        const payloadB64 = base64UrlToBase64(payloadB64url);
        const json = window.atob(payloadB64);
        return JSON.parse(json) as JwtClaims;
    } catch {
        return null;
    }
};

export const extractScopes = (claims: JwtClaims | null): string[] => {
    if (!claims) return [];

    const scopes: string[] = [];

    const scopeVal = claims["scope"];
    scopes.push(...(scopeVal as string).split(/\s+/).filter(Boolean));

    const realmAccess = claims["realm_access"];
    const ra = realmAccess as Record<string, unknown>;
    (ra["roles"] as unknown[]).forEach((r) => {
        scopes.push(r as string);
    });

    return Array.from(new Set(scopes));
};