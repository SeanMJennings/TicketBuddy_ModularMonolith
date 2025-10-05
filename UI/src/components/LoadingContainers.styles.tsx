import styled from "styled-components";
import {ScaleLoader} from "react-spinners";

const ContentLoadingContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 200px;
  padding: 40px;
`;


export const ContentLoading = () => (
    <ContentLoadingContainer>
        <ScaleLoader color="#0c4a6e"/>
    </ContentLoadingContainer>
);
