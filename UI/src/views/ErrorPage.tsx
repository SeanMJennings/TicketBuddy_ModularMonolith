import React from "react";

export const ErrorPage: React.FC = () => {
  return (
    <div style={{ padding: 32, textAlign: "center" }}>
      <h1>Something went wrong</h1>
      <p>We're sorry, but an unexpected error occurred. Please try again later.</p>
    </div>
  );
};