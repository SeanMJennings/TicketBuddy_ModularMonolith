import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

export const userManager = new UserManager({
  authority: import.meta.env.VITE_AUTHORITY,
  client_id: import.meta.env.VITE_CLIENT_ID,
  redirect_uri: `${window.location.origin}${window.location.pathname}`,
  post_logout_redirect_uri: window.location.origin,
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
  monitorSession: true,
});

export const onSigninCallback = () => {
  window.history.replaceState({}, document.title, window.location.origin);
};

export const onSignoutCallback = () => {
    window.history.replaceState({}, document.title, window.location.origin);
}
