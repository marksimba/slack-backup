import React from 'react';
import { Redirect, Route, Switch } from 'react-router-dom';
import './App.css';
import URLS from './constants/urls';
import { connect } from 'react-redux';

/** Custom Components */
import Authentication from './components/Authentication/Authentication';
import Slack from './components/Slack/Slack';



const App = ({
  authenticated
}) => {

  const PrivateRoute = ({ component: Component, ...rest }) => {
    return authenticated ? <Component {...rest}/> : <Redirect to="/login"/>;
  };

  return (
    <Switch>
      <Route path="/login" component={ () => {
        window.location.href = URLS.auth;
        return null;
      }}/>
      <Route path="/authentication" component={() => { return (<Authentication/>)}} />
      <PrivateRoute path="/" component={() => { return (<Slack/>)}} />
    </Switch>
  );
}

const mapStateToProps = state => ({
  authenticated: state.credentials.authenticated
});

export default connect(
  mapStateToProps,
  null
)(App);