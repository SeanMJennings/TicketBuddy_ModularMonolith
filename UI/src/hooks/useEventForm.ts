import React, { useState, useEffect, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "react-oidc-context";
import { toast } from "react-toastify";
import moment from "moment";
import { getEventById, postEvent, putEvent } from "../api/events.api";
import { getVenues } from "../api/venues.api";
import { type Venue } from "../domain/venue";

type EventFormData = {
    eventName: string;
    startDateTime: string;
    endDateTime: string;
    venueId: string;
    price: number;
};

const initialFormData: EventFormData = {
    eventName: '',
    startDateTime: '',
    endDateTime: '',
    venueId: '',
    price: 0,
};

type UseEventFormResult = {
    formData: EventFormData;
    venues: Venue[];
    loading: boolean;
    isEditMode: boolean;
    handleInputChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    handleVenueChange: (e: React.ChangeEvent<HTMLSelectElement>) => void;
    handleSubmit: (e: React.FormEvent) => void;
    isFormValid: () => boolean;
};

type UseEventFormOptions = {
    mode: 'create' | 'edit';
};

export const useEventForm = ({ mode }: UseEventFormOptions): UseEventFormResult => {
    const [formData, setFormData] = useState<EventFormData>(initialFormData);
    const [venues, setVenues] = useState<Venue[]>([]);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEditMode = mode === 'edit';
    const auth = useAuth();

    useEffect(() => {
        getVenues().then(setVenues).catch(() => {});
    }, []);

    useEffect(() => {
        if (isEditMode && id) {
            setLoading(true);
            getEventById(id).then(event => {
                setFormData({
                    eventName: event.EventName,
                    startDateTime: moment(event.StartDate).format('YYYY-MM-DDTHH:mm'),
                    endDateTime: moment(event.EndDate).format('YYYY-MM-DDTHH:mm'),
                    venueId: event.VenueId,
                    price: event.Price,
                });
                setLoading(false);
            }).catch(() => {
                toast.error('Failed to fetch event details');
                setLoading(false);
                navigate('/events-management');
            });
        }
    }, [id, isEditMode, navigate]);

    const handleInputChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    }, []);

    const handleVenueChange = useCallback((e: React.ChangeEvent<HTMLSelectElement>) => {
        setFormData(prev => ({
            ...prev,
            venueId: e.target.value
        }));
    }, []);

    const isFormValid = useCallback(() => {
        return Boolean(formData.eventName && formData.startDateTime && formData.endDateTime && formData.venueId);
    }, [formData.eventName, formData.startDateTime, formData.endDateTime, formData.venueId]);

    const handleSubmit = useCallback((e: React.FormEvent) => {
        e.preventDefault();
        if (isFormValid()) {
            const eventData = {
                EventName: formData.eventName,
                StartDate: new Date(formData.startDateTime).toISOString(),
                EndDate: new Date(formData.endDateTime).toISOString(),
                Price: formData.price,
            };

            const apiCall = isEditMode && id
                ? putEvent(id, eventData, auth.user?.access_token ?? '')
                : postEvent({ ...eventData, VenueId: formData.venueId }, auth.user?.access_token ?? '');

            apiCall.then(() => {
                setFormData(initialFormData);
                navigate('/events-management');
            }).catch((error) => {
                if (error.errors && Array.isArray(error.errors)) {
                    error.errors.forEach((errorMessage: string) => {
                        toast.error(errorMessage);
                    });
                } else {
                    toast.error(`Failed to ${isEditMode ? 'update' : 'create'} event`);
                }
            });
        }
    }, [formData, isEditMode, id, auth.user?.access_token, navigate, isFormValid]);

    return {
        formData,
        venues,
        loading,
        isEditMode,
        handleInputChange,
        handleVenueChange,
        handleSubmit,
        isFormValid
    };
};
