import styled from "styled-components";
import {ScaleLoader} from "react-spinners";

export const LoadingContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background: rgba(0, 0, 0, 0.8);
`;

export const Loading = () => (
    <LoadingContainer>
        <ScaleLoader color="#0c4a6e"/>
    </LoadingContainer>
);