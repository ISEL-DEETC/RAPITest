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
            dictionary: null,
            dllFiles: null,
            timeSpecification: null,
            errorMessage: ""
        }

        this.handlerName = this.handlerName.bind(this)
        this.handlerAPI = this.handlerAPI.bind(this)
        this.handlerTest = this.handlerTest.bind(this)
        this.handlerTime = this.handlerTime.bind(this)
        this.sendTestSetup = this.sendTestSetup.bind(this)
        this.restartCallback = this.restartCallback.bind(this)
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

    handlerTest(tsl,dictionary,dll) {
        this.setState({
            testSpecification: tsl,
            dictionary: dictionary,
            dllFiles: dll,
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
        data.append('dictionary.txt', this.state.dictionary);
        let i = 1
        for (const file of this.state.testSpecification) {
            data.append("tsl_"+i+".yaml", file)
            i++
        }
        for (const file of this.state.dllFiles) {
            console.log(file)
            data.append(file.name, file)
        }
        data.append('name', this.state.name);
        data.append('runimmediately', this.state.timeSpecification.runimmediately);
        data.append('interval', this.state.timeSpecification.interval);

        const token = await authService.getAccessToken();
        fetch(`SetupTest/UploadFile`, {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
            body: data
        }).then(res => {
            if (!res.ok) {
                res.text().then(text => this.setState({ step: 6, errorMessage: text}))
            } else {
                this.setState({ step: 5 })
            }
        })
   
    }

    restartCallback() {
        this.setState({ step: 1 })
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
            case 5:
                return <h3>Success! Go over to <a href="/monitorTests">Monitor Tests</a> to check the results or  <button type="button" className="btn btn-outline-primary" onClick={this.restartCallback}>press here</button> to setup another test.</h3>
            default:
                return <div><h3>Error!</h3> <h4>{this.state.errorMessage}</h4> <button type="button" className="btn btn-outline-primary" onClick={this.restartCallback}>Restart</button> </div>
        }
    }

    /* render MultiStep */
    render() {
        return (
            <div>    
                {this.state.step < 5 && <h1>Step {this.state.step}</h1>}
                {this.renderSwitch()}
            </div>
        );
    }
}