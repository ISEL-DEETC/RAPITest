import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap';

import { UploadApiSpecification } from "./steps/UploadApiSpecification";
import { UploadTestSpecification } from "./steps/UploadTestSpecification";
import { TimeLoop } from "./steps/TimeLoop";


export class SetupTest extends Component {

    constructor() {

        super()

        this.state = {
            step: 1
        }

        this.handler = this.handler.bind(this)
    }

    handler(nextStep) {
        this.setState({
            step: nextStep
        })
    }

    renderSwitch() {
        switch (this.state.step) {
            case 1:
                return <UploadApiSpecification handler={this.handler} /> ;
            case 2:
                return <UploadTestSpecification handler={this.handler}/>;
            case 3:
                return <TimeLoop handler={this.handler} />;
            default:
                return <UploadApiSpecification handler={this.handler}/>;
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