import React from 'react';
import { render } from '@testing-library/react';
import App from './App';
import rootReducer from './reducers'
import { createStore, applyMiddleware } from 'redux';
import thunk from 'redux-thunk';
import { createBrowserHistory } from 'history';
import { Provider } from 'react-redux';
import { Router } from 'react-router-dom';

const store = createStore( rootReducer, applyMiddleware( thunk ) );
const history = createBrowserHistory();


it('renders without crashing', () => {
  render(
    <Provider store={store}>
        <Router history={history}>
            <App />
        </Router>
    </Provider>
  );
});
