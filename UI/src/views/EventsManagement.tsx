import React, {useEffect, useState} from 'react';
import {
    AddIcon,
    BackIcon,
    EventContent,
    EventActions,
    FormContainer,
    FormGroup,
    Input,
    Label,
    Select,
} from './EventsManagement.styles.tsx';
import {ConvertVenueToString, type Event, Venue} from '../domain/event.ts';
import {getEventById, getEvents, postEvent, putEvent,} from "../api/events.api.ts";
import moment from 'moment'
import {Link, Outlet, Route, Routes, useNavigate, useParams} from "react-router-dom";
import {Button} from "../components/Button.styles.tsx";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import {EventItem, EventList, PageTitle, PageContainer, ActionBar, Container} from "./Common.styles.tsx";
import {ContentLoading} from "../components/LoadingContainers.styles.tsx";

type EventFormData = {
    eventName: string;
    startDateTime: string;
    endDateTime: string;
    venue: Venue;
    price: number;
};

const initialFormData: EventFormData = {
    eventName: '',
    startDateTime: '',
    endDateTime: '',
    venue: Venue.O2ArenaLondon,
    price: 0,
};

export const EventsManagement = () => {
    return (
        <Container>
            <Routes>
                <Route index element={<ListEvents />} />
                <Route path="add" element={<EventForm mode="create" />} />
                <Route path="edit/:id" element={<EventForm mode="edit" />} />
            </Routes>
            <Outlet />
        </Container>
    );
}

export const ListEvents = () => {
    const [events, setEvents] = useState<Event[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getEvents().then(data => {
            setEvents(data);
            setLoading(false);
        }).catch(() => {
            setLoading(false);
        });
    },[]);

    return (
        <>
            <PageTitle>Events Management</PageTitle>
            <PageContainer>
                <ActionBar>
                    <Link to="add">
                        <Button>
                            Add Event <AddIcon/>
                        </Button>
                    </Link>
                </ActionBar>
                {loading ? (
                    <ContentLoading />
                ) : (
                    <EventList>
                        {events.map((event, index) => (
                            <EventItem key={index} className="event-item">
                                <EventContent>
                                    <h2>{event.EventName}</h2>
                                    <p>{moment(event.StartDate).format('MMMM Do YYYY, h:mm A')} to {moment(event.EndDate).format('MMMM Do YYYY, h:mm A')}</p>
                                    <p>Venue: {ConvertVenueToString(event.Venue)}</p>
                                </EventContent>
                                <EventActions>
                                    <Link to={`edit/${event.Id}`}>
                                        <Button data-testid={`edit-event-${event.EventName}`}>
                                            Edit Event
                                        </Button>
                                    </Link>
                                </EventActions>
                            </EventItem>
                        ))}
                    </EventList>
                )}
            </PageContainer>
        </>
    );
}

interface EventFormProps {
    mode: 'create' | 'edit';
}

export const EventForm = ({ mode }: EventFormProps) => {
    const [formData, setFormData] = useState<EventFormData>(initialFormData);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEditMode = mode === 'edit';

    useEffect(() => {
        if (isEditMode && id) {
            setLoading(true);
            const fetchEventDetails = async () => {
                getEventById(id).then(event => {
                setFormData({
                    eventName: event.EventName,
                    startDateTime: moment(event.StartDate).format('YYYY-MM-DDTHH:mm'),
                    endDateTime: moment(event.EndDate).format('YYYY-MM-DDTHH:mm'),
                    venue: event.Venue,
                    price: event.Price,
                });
                setLoading(false);
                }).catch(() => {
                    toast.error('Failed to fetch event details');
                    setLoading(false);
                    navigate('/events-management');
                });
            };

            fetchEventDetails();
        }
    }, [id, isEditMode, navigate]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (isFormValid()) {
            const eventData = {
                EventName: formData.eventName,
                StartDate: moment(formData.startDateTime),
                EndDate: moment(formData.endDateTime),
                Price: formData.price,
            };

            const apiCall = isEditMode && id
                ? putEvent(id, eventData)
                : postEvent({ ...eventData, Venue: formData.venue });

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
    };

    const isFormValid = () => {
        return formData.eventName && formData.startDateTime && formData.endDateTime && formData.venue;
    };

    return (
        <>
            <PageTitle>Events Management</PageTitle>
            <Link to="/events-management">
                <Button data-testid="back-button">
                    <BackIcon /> Back to Events
                </Button>
            </Link>
            {loading ? (
                <ContentLoading />
            ) : (
                <FormContainer
                    data-testid={isEditMode ? "event-update-form" : "event-creation-form"}
                    onSubmit={handleSubmit}
                >
                    <h2>{isEditMode ? 'Edit Event' : 'Create New Event'}</h2>

                    <FormGroup>
                        <Label htmlFor="eventName">Event Name</Label>
                        <Input
                            type="text"
                            id="eventName"
                            name="eventName"
                            value={formData.eventName}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="startDateTime">Start Date</Label>
                        <Input
                            type="datetime-local"
                            id="startDateTime"
                            name="startDateTime"
                            value={formData.startDateTime}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="endDateTime">End Date</Label>
                        <Input
                            type="datetime-local"
                            id="endDateTime"
                            name="endDateTime"
                            value={formData.endDateTime}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="venue">Venue</Label>
                        <Select
                            id="venue"
                            name="venue"
                            value={formData.venue}
                            onChange={(e) => setFormData({ ...formData, venue: e.target.value as Venue })}
                            disabled={isEditMode}
                        >
                            {Object.values(Venue).map((venue) => (
                                <option key={venue} value={venue}>
                                    {ConvertVenueToString(venue)}
                                </option>
                            ))}
                        </Select>
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="price">Ticket Price (£)</Label>
                        <Input
                            type="number"
                            id="price"
                            name="price"
                            value={formData.price}
                            onChange={handleInputChange}
                            placeholder="Enter ticket price to release tickets"
                            step="0.01"
                        />
                    </FormGroup>

                    <Button
                        type="submit"
                        data-testid={isEditMode ? "update-event-button" : "create-event-button"}
                        disabled={!isFormValid()}
                    >
                        {isEditMode ? 'Update Event' : 'Create Event'}
                    </Button>
                </FormContainer>
            )}
        </>
    );
};
