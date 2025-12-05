import * as signalR from '@microsoft/signalr';
import { DEFAULT_URLS, STORAGE_KEYS } from '../constants';

const SIGNALR_URL = import.meta.env.VITE_SIGNALR_URL || DEFAULT_URLS.SIGNALR_URL;

export const connection = new signalR.HubConnectionBuilder()
  .withUrl(SIGNALR_URL, {
    accessTokenFactory: () => localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN) || '',
  })
  .withAutomaticReconnect()
  .build();

export const startConnection = async () => {
  if (connection.state === 'Disconnected') {
    try {
      await connection.start();
      console.log('SignalR Connected');
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      setTimeout(startConnection, 5000);
    }
  }
};

export const stopConnection = async () => {
  try {
    await connection.stop();
    console.log('SignalR Disconnected');
  } catch (err) {
    console.error('SignalR Disconnect Error:', err);
  }
};
