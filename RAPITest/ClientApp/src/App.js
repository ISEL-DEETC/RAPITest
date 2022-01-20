import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './pages/Layout';
import { Home } from './pages/Home';
import { SetupTest } from './pages/SetupTest';
import { MonitorTest } from './pages/MonitorTest';
import AuthorizeRoute from './pages/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './pages/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './pages/api-authorization/ApiAuthorizationConstants';


import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <AuthorizeRoute path='/setupTest' component={SetupTest} />
        <AuthorizeRoute path='/monitorTests' component={MonitorTest} />
        <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </Layout>
    );
  }
}
