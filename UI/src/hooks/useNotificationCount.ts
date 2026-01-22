import { useState, useEffect, useCallback } from "react";
import { useAuth } from "react-oidc-context";
import { getUnreadCount } from "../api/notifications.api";

const POLL_INTERVAL_MS = 30000;

type UseNotificationCountResult = {
    count: number | null;
    isLoading: boolean;
    error: Error | null;
    refetch: () => void;
};

export const useNotificationCount = (): UseNotificationCountResult => {
    const auth = useAuth();
    const [count, setCount] = useState<number | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    const isAuthenticated = auth.isAuthenticated && auth.user?.access_token;

    const fetchCount = useCallback(async () => {
        if (!isAuthenticated || !auth.user?.access_token) {
            setIsLoading(false);
            return;
        }

        try {
            const result = await getUnreadCount(auth.user.access_token);
            setCount(result.Count);
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err : new Error("Failed to fetch notification count"));
        } finally {
            setIsLoading(false);
        }
    }, [isAuthenticated, auth.user?.access_token]);

    useEffect(() => {
        if (!isAuthenticated) {
            setIsLoading(false);
            return;
        }

        fetchCount();

        const intervalId = setInterval(fetchCount, POLL_INTERVAL_MS);

        return () => {
            clearInterval(intervalId);
        };
    }, [isAuthenticated, fetchCount]);

    return {
        count,
        isLoading,
        error,
        refetch: fetchCount
    };
};
