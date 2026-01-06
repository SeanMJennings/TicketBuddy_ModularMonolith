import React, { useEffect, useState } from "react";
import {
    AddIcon,
    BackIcon,
    EventContent,
    EventActions,
    FormContainer,
    FormGroup,
    Input,
    Label,
} from "./EventsManagement.styles.tsx";
import { type Venue, type VenuePayload } from "../domain/venue.ts";
import { getVenues, getVenueById, createVenue, updateVenue } from "../api/venues.api.ts";
import { Link, Outlet, Route, Routes, useNavigate, useParams } from "react-router-dom";
import { Button } from "../components/Button.styles.tsx";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { EventItem, EventList, PageTitle, PageContainer, ActionBar, Container } from "./Common.styles.tsx";
import { ContentLoading } from "../components/LoadingContainers.styles.tsx";
import { useAuth } from "react-oidc-context";

type VenueFormData = {
    name: string;
    street: string;
    city: string;
    postCode: string;
    capacity: number;
};

const initialFormData: VenueFormData = {
    name: "",
    street: "",
    city: "",
    postCode: "",
    capacity: 1,
};

export const VenuesManagement = () => {
    return (
        <Container>
            <Routes>
                <Route index element={<ListVenues />} />
                <Route path="add" element={<VenueForm mode="create" />} />
                <Route path="edit/:id" element={<VenueForm mode="edit" />} />
            </Routes>
            <Outlet />
        </Container>
    );
};

export const ListVenues = () => {
    const [venues, setVenues] = useState<Venue[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getVenues()
            .then((data) => {
                setVenues(data);
                setLoading(false);
            })
            .catch(() => {
                setLoading(false);
            });
    }, []);

    return (
        <>
            <PageContainer>
                <PageTitle>Venues Management</PageTitle>
                <ActionBar>
                    <Link to="add">
                        <Button>
                            Add Venue <AddIcon />
                        </Button>
                    </Link>
                </ActionBar>
                {loading ? (
                    <ContentLoading />
                ) : (
                    <EventList>
                        {venues.map((venue, index) => (
                            <EventItem key={index} className="venue-item">
                                <EventContent>
                                    <h2>{venue.Name}</h2>
                                    <p>
                                        {venue.Address.Street}, {venue.Address.City}, {venue.Address.Postcode}
                                    </p>
                                    <p>Capacity: {venue.Capacity}</p>
                                </EventContent>
                                <EventActions>
                                    <Link to={`edit/${venue.Id}`}>
                                        <Button data-testid={`edit-venue-${venue.Name}`}>Edit Venue</Button>
                                    </Link>
                                </EventActions>
                            </EventItem>
                        ))}
                    </EventList>
                )}
            </PageContainer>
        </>
    );
};

type VenueFormProps = {
    mode: "create" | "edit";
};

export const VenueForm = ({ mode }: VenueFormProps) => {
    const [formData, setFormData] = useState<VenueFormData>(initialFormData);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { id } = useParams<{ id: string }>();
    const isEditMode = mode === "edit";
    const auth = useAuth();

    useEffect(() => {
        if (isEditMode && id) {
            setLoading(true);
            getVenueById(id)
                .then((venue) => {
                    setFormData({
                        name: venue.Name,
                        street: venue.Address.Street,
                        city: venue.Address.City,
                        postCode: venue.Address.Postcode,
                        capacity: venue.Capacity,
                    });
                    setLoading(false);
                })
                .catch(() => {
                    toast.error("Failed to fetch venue details");
                    setLoading(false);
                    navigate("/venues-management");
                });
        }
    }, [id, isEditMode, navigate]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value,
        });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (isFormValid()) {
            const venuePayload: VenuePayload = {
                Name: formData.name,
                Street: formData.street,
                City: formData.city,
                Postcode: formData.postCode,
                Capacity: Number(formData.capacity),
            };

            const apiCall =
                isEditMode && id
                    ? updateVenue(id, venuePayload, auth.user?.access_token ?? "")
                    : createVenue(venuePayload, auth.user?.access_token ?? "");

            apiCall
                .then(() => {
                    setFormData(initialFormData);
                    navigate("/venues-management");
                })
                .catch((error) => {
                    if (error.errors && Array.isArray(error.errors)) {
                        error.errors.forEach((errorMessage: string) => {
                            toast.error(errorMessage);
                        });
                    } else {
                        toast.error(`Failed to ${isEditMode ? "update" : "create"} venue`);
                    }
                });
        }
    };

    const isFormValid = () => {
        return formData.name && formData.street && formData.city && formData.postCode && formData.capacity > 0;
    };

    return (
        <>
            <PageTitle>Venues Management</PageTitle>
            <Link to="/venues-management">
                <Button data-testid="back-button">
                    <BackIcon /> Back to Venues
                </Button>
            </Link>

            {loading ? (
                <ContentLoading />
            ) : (
                <FormContainer
                    data-testid={isEditMode ? "venue-update-form" : "venue-creation-form"}
                    onSubmit={handleSubmit}
                >
                    <h2>{isEditMode ? "Edit Venue" : "Create New Venue"}</h2>

                    <FormGroup>
                        <Label htmlFor="name">Venue Name</Label>
                        <Input
                            type="text"
                            id="name"
                            name="name"
                            value={formData.name}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="street">Street</Label>
                        <Input
                            type="text"
                            id="street"
                            name="street"
                            value={formData.street}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="city">City</Label>
                        <Input
                            type="text"
                            id="city"
                            name="city"
                            value={formData.city}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="postCode">Post Code</Label>
                        <Input
                            type="text"
                            id="postCode"
                            name="postCode"
                            value={formData.postCode}
                            onChange={handleInputChange}
                        />
                    </FormGroup>

                    <FormGroup>
                        <Label htmlFor="capacity">Capacity</Label>
                        <Input
                            type="number"
                            id="capacity"
                            name="capacity"
                            value={formData.capacity}
                            onChange={handleInputChange}
                            min="1"
                            max="50"
                        />
                    </FormGroup>

                    <Button
                        type="submit"
                        data-testid={isEditMode ? "update-venue-button" : "create-venue-button"}
                        disabled={!isFormValid()}
                    >
                        {isEditMode ? "Update Venue" : "Create Venue"}
                    </Button>
                </FormContainer>
            )}
        </>
    );
};
