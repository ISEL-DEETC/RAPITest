import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap';
import { Row, Col, Figure } from 'react-bootstrap'
import { TestName } from "./steps/TestName";
import { UploadApiSpecification } from "./steps/UploadApiSpecification";
import { UploadTestSpecification } from "./steps/UploadTestSpecification";
import { TimeLoop } from "./steps/TimeLoop";
import Steps from "rc-steps";
import authService from './api-authorization/AuthorizeService';
import "rc-steps/assets/index.css";
import './iconfront.css';
import successIcon from '../assets/tick.png'
import errorIcon from '../assets/remove.png'
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";


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
        this.goToMonitorTests = this.goToMonitorTests.bind(this)
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
        if (this.state.dictionary !== null) {
            data.append('dictionary.txt', this.state.dictionary);
        }
        let i = 1
        for (const file of this.state.testSpecification) {
            data.append("tsl_"+i+".yaml", file)
            i++
        }
        if (this.state.dllFiles !== null) {
            for (const file of this.state.dllFiles) {
                console.log(file)
                data.append(file.name, file)
            }
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

    renderSuccess(title,message,icon) {
        return (
            <Row>
                <Col sm={4}>
                    <Row>
                        {title}
                    </Row>
                    <Row>
                        {message}
                    </Row>
                </Col>
                <Col sm={8}>
                    <Figure style={{ padding: "100px 0px 0px 250px" }}>
                        <Figure.Image
                            width={300}
                            height={300}
                            alt="300x300"
                            src={icon}
                        />
                        <Figure.Caption>
                        </Figure.Caption>
                    </Figure>
                </Col>
            </Row>
        )
    }

    goToMonitorTests() {
        this.props.history.push(`monitorTests`)
    }

    renderSwitch() {
        let title = ""
        let message = ""
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
                title = <div style={{ textAlign: "center", fontSize: "35px", fontWeight: "bold" }}>Success!</div>
                message = <div style={{ paddingTop: "50px", textAlign: "center", fontSize: "30px" }}><AwesomeButton type="primary" onPress={this.goToMonitorTests}>Check Results</AwesomeButton><br></br>or<br></br><AwesomeButton type="primary" onPress={this.restartCallback}>Setup Another Test</AwesomeButton></div>
                return this.renderSuccess(title,message,successIcon)
            default:
                title = <div style={{ textAlign: "center", fontSize: "35px", fontWeight: "bold" }}>Error!</div>
                message = <div style={{ paddingTop: "50px", textAlign: "center", fontSize: "20px" }}>{this.state.errorMessage}<br></br><AwesomeButton type="primary" onPress={this.restartCallback}>Setup Another Test</AwesomeButton></div>
                return this.renderSuccess(title, message, errorIcon)
        }
    }

    /* render MultiStep */
    render() {
        return (
            <div>
                <Row style={{paddingTop: "25px"}}>
                    <Steps labelPlacement="horizontal" current={this.state.step-1}>
                        <Steps.Step title="Test Name" description="Configure the test name, usually the API name" />
                        <Steps.Step title="API Specification" description="Browse or Drag'n'Drop to upload the API's specification" />
                        <Steps.Step title="TSL files" description="Optionally Upload TSL files to customize your tests" />
                        <Steps.Step title="Timer" description="Choose a timer for the automatic repetition of your tests" />
                        <Steps.Step title="Done!" description="Setup another test, or monitor your configured tests" />
                    </Steps>
                </Row>
                <Row style={{paddingTop: "50px"}}>
                    {this.renderSwitch()}
                </Row>
            </div>
        );
    }
}