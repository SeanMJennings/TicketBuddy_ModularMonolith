import styled from "styled-components";

const NotFoundContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 200px;
  padding: 40px;
`;

export const NotFound = () => {
    return (
        <NotFoundContainer>
            <h1>Page not found</h1>
        </NotFoundContainer>
    );
}