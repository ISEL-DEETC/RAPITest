import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap';

import { TestName } from "./steps/TestName";
import { UploadApiSpecification } from "./steps/UploadApiSpecification";
import { UploadTestSpecification } from "./steps/UploadTestSpecification";
import { TimeLoop } from "./steps/TimeLoop";

import authService from './api-authorization/AuthorizeService';


export class SetupTest extends Component {

    constructor() {

        super()

        this.state = {
            step: 1,
            name: "",
            apiSpecification: null,
            testSpecification: null,
            timeSpecification: null
        }

        this.handlerName = this.handlerName.bind(this)
        this.handlerAPI = this.handlerAPI.bind(this)
        this.handlerTest = this.handlerTest.bind(this)
        this.handlerTime = this.handlerTime.bind(this)
        this.sendTestSetup = this.sendTestSetup.bind(this)
    }

    handlerName(nameTest) {
        this.setState({
            name: nameTest,
            step:2
        })
    }

    handlerAPI(api) {
        this.setState({
            apiSpecification: api,
            step:3
        })
    }

    handlerTest(test) {
        this.setState({
            testSpecification: test,
            step:4
        })
    }

    handlerTime(time) {
        this.setState({
            timeSpecification: time
        },() => this.sendTestSetup())
    }

    async sendTestSetup() {

        let data = new FormData();
        data.append('apiSpecification', this.state.apiSpecification);
        data.append('TSL', this.state.testSpecification);
        data.append('name', this.state.name);
        data.append('runimmediately', this.state.timeSpecification.runimmediately);
        data.append('interval', this.state.timeSpecification.interval);

        const token = await authService.getAccessToken();
        fetch(`SetupTest/UploadFile`, {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
            body: data
        }).then(res => {
            console.log(res)
            this.setState({step:5})
        })
   
    }

    renderSwitch() {
        switch (this.state.step) {
            case 1:
                return <TestName handlerName={this.handlerName} />;
            case 2:
                return <UploadApiSpecification handlerAPI={this.handlerAPI} /> ;
            case 3:
                return <UploadTestSpecification handlerTest={this.handlerTest}/>;
            case 4:
                return <TimeLoop handlerTime={this.handlerTime} />;
            default:
                return <h3>Success! Go over to <a href="/monitorTests">Monitor Tests</a> to check the results</h3>
        }
    }

    /* render MultiStep */
    render() {
        return (
            <div>    
                <h1>Step {this.state.step}</h1>
                {this.renderSwitch()}
            </div>
        );
    }
}