import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import "react-awesome-button/dist/styles.css";
import CircleWorkflowComp from '../../components/CircleWorkflowComp.js'

export default class GeneratedTests extends Component {


    render() {
        let fullGeneratedTests = this.props.fullGeneratedTests
        let clickableFunction = this.props.clickableFunction

        return (
            <div>
                <h3 style={{ paddingTop: "20px", paddingBottom: "20px", fontWeight: "bold" }}>Generated Tests</h3>
                <CircleWorkflowComp
                    workflow={fullGeneratedTests}
                    indexWorkflow={0}
                    clickableFunction={clickableFunction}
                    fromGenerated={true}
                />
            </div>
        )
    }
}