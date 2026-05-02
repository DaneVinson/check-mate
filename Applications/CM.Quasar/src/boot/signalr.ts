// Boot file: SignalR connection to /events hub
// TODO: configure @microsoft/signalr HubConnection and wire to Pinia stores
// import { boot } from '#q-app/wrappers';
// import * as signalR from '@microsoft/signalr';

// export default boot(() => {
//   const connection = new signalR.HubConnectionBuilder()
//     .withUrl('/events', { accessTokenFactory: () => authStore.accessToken ?? '' })
//     .withAutomaticReconnect()
//     .build();
//
//   connection.on('ReceiveEvent', (envelope: { type: string; payload: unknown }) => {
//     // dispatch to appropriate store based on envelope.type
//   });
//
//   void connection.start();
// });
