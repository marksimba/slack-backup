import { combineReducers } from 'redux';
import app from './app';
import credentials from './credentials';

export default combineReducers({
    app,
    credentials
});