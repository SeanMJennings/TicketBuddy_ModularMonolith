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
import moment from 'moment'
import {Link, Outlet, Route, Routes} from "react-router-dom";
import {Button} from "../components/Button.styles.tsx";
import "react-toastify/dist/ReactToastify.css";
import {EventItem, EventList, PageTitle, PageContainer, ActionBar, Container} from "./Common.styles.tsx";
import {ContentLoading} from "../components/LoadingContainers.styles.tsx";
import {VenueDisplay} from "../components/VenueDisplay.tsx";
import {useEventsListData} from "../hooks/useEventsListData";
import {useEventForm} from "../hooks/useEventForm";

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
    const { events, venues, loading } = useEventsListData();

    return (
        <>
            <PageContainer>
                <PageTitle>Events Management</PageTitle>
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
                                    <p>Venue: <VenueDisplay venues={venues} venueId={event.VenueId} /></p>
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
    const {
        formData,
        venues,
        loading,
        isEditMode,
        handleInputChange,
        handleVenueChange,
        handleSubmit,
        isFormValid
    } = useEventForm({ mode });

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
                        <Label htmlFor="venueId">Venue</Label>
                        <Select
                            id="venueId"
                            name="venueId"
                            value={formData.venueId}
                            onChange={handleVenueChange}
                            disabled={isEditMode}
                        >
                            <option value="">Select a venue</option>
                            {venues.map((venue) => (
                                <option key={venue.Id} value={venue.Id}>
                                    {venue.Name}
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
