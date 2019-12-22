import React, { useEffect } from 'react';
import { Redirect } from 'react-router-dom';
import { CognitoIdentityCredentials } from 'aws-sdk';
import COGNITO from '../../constants/cognito';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { getAWSCognitoTokens } from '../../services/network';
import { setUrlParams } from '../../actions/app';
import { setAWSCognitoCredentials } from '../../actions/credentials';
import './Authentication.css';

/* React Spinners */
import "react-loader-spinner/dist/loader/css/react-spinner-loader.css"
import Loader from 'react-loader-spinner'

const getCognitoIdentityCredentials = ( token ) => {
  
  const poolUrl = `${COGNITO.userPoolBaseUri}${COGNITO.userPool}`

  const login = {};
  login[poolUrl] = token

  return  new CognitoIdentityCredentials({
                                            IdentityPoolId: COGNITO.IdentityPoolId,
                                            Logins: login
                                        })
}


const Authentication = ({
  setUrlParams,
  setAWSCognitoCredentials,
  authenticated
}) => {

  // Retrieves parameter string, and parses to an object. 
  // If 'code' exists as a parameter, retrieve AWS credentials.

  useEffect( () =>{
    const paramsString = window.location.href.split("?")[1];
    if( paramsString ){

      let newParams = {};
      paramsString.replace("?","").split("&").map(value => {
          let ary = value.split("=")
          newParams[ary[0]] = ary[1]
          return null;
      });

      if ( newParams['code'] ) {
        getAWSCognitoTokens( newParams['code'] )
          .then(resp => {
            const { id_token } = resp;
            if( id_token ){
              setAWSCognitoCredentials(getCognitoIdentityCredentials(id_token));
            }
          })
          .catch(err => console.error(err))
      }
      setUrlParams(newParams);
    }
  })

  
  return (
    authenticated ?
    <Redirect to="/"/> :
      <div className='center'>
        Authenticating
        <br/>
        <br/>
        <Loader 
          type='MutatingDots'
          color="#00BFFF"
          height={100}
          width={100}
        />
      </div>
  );
}

const mapStateToProps = state => ({
  authenticated: state.credentials.authenticated
});

const mapDispatchToProps = dispatch => ({
  setUrlParams: bindActionCreators(setUrlParams, dispatch),
  setAWSCognitoCredentials: bindActionCreators(setAWSCognitoCredentials, dispatch)
});

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Authentication);