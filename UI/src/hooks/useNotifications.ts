import { useState, useEffect, useCallback } from "react";
import { useAuth } from "react-oidc-context";
import { getNotifications } from "../api/notifications.api";
import type { Notification } from "../domain/notification";

type UseNotificationsResult = {
    notifications: Notification[];
    isLoading: boolean;
    error: Error | null;
    refetch: () => void;
};

export const useNotifications = (): UseNotificationsResult => {
    const auth = useAuth();
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<Error | null>(null);

    const isAuthenticated = auth.isAuthenticated && auth.user?.access_token;

    const fetchNotifications = useCallback(async () => {
        try {
            const result = await getNotifications(auth.user?.access_token ?? '');
            setNotifications(result);
            setError(null);
        } catch (err) {
            setError(err instanceof Error ? err : new Error("Failed to fetch notifications"));
        } finally {
            setIsLoading(false);
        }
    }, [auth.user?.access_token]);

    useEffect(() => {
        fetchNotifications();
    }, [isAuthenticated, fetchNotifications]);

    return {
        notifications,
        isLoading,
        error,
        refetch: fetchNotifications
    };
};
