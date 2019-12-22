import React from 'react';
import { render } from '@testing-library/react';
import Reports from './Reports';
import { createStore, applyMiddleware } from 'redux';
import thunk from 'redux-thunk';
import { Provider } from 'react-redux';
import rootReducer from '../../../reducers'
const store = createStore( rootReducer, applyMiddleware( thunk ) );

it('renders without crashing', () => {
  render(
    <Provider store={store}>
        <Reports />
    </Provider>
  );
});
