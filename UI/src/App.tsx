import {Route, Routes} from "react-router-dom";
import {Home} from "./views/Home.tsx";
import {ToastContainer} from "react-toastify";
import {Header} from "./components/Header.tsx";
import {MainContainer} from "./components/MainContainer.styles.tsx";
import {useUsersStore} from "./stores/users.store.ts";
import {useShallow} from "zustand/react/shallow";
import {useEffect} from "react";
import {EventsManagement} from "./views/EventsManagement.tsx";
import {NotFound} from "./components/NotFound.tsx";
import {Tickets} from "./views/Tickets.tsx";
import {TicketPurchase} from "./views/TicketPurchase.tsx";
import {UserProfile} from "./views/UserProfile.tsx";

function App() {
    const { fetchUsers } = useUsersStore(useShallow((state => ({
        fetchUsers: state.fetchUsers
    }))));

    useEffect(() => {
        fetchUsers();
    }, [fetchUsers]);

  return (
      <>
          <Header />
          <Main/>
      </>
  );
}

export const Main = () => {
    return (
        <MainContainer>
            <AppRoutes />
            <ToastContainer />
        </MainContainer>
    );
}

export const AppRoutes = () => (
    <Routes>
        <Route path="/events-management/*" element={<EventsManagement />} />
        <Route path="/tickets/:eventId" element={<Tickets />} />
        <Route path="/tickets/:eventId/purchase" element={<TicketPurchase />} />
        <Route path="/profile" element={<UserProfile />} />
        <Route path="/" element={<Home/>} />
        <Route path="*" element={<NotFound/>}/>
    </Routes>
);

export default App