import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Dropzone from '../../components/Dropzone'
import { Container, Row, Col, Figure} from 'react-bootstrap'
import './UploadFile.css';
import { warningMessage } from '../../components/AlertComp'
import ListGroupComp from '../../components/ListGroupComp'
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import InfoIcon from '../../assets/info.png'
import backIcon from '../../assets/back.png'
import createIcon from '../../assets/optimization.png'
import uploadIcon from '../../assets/uploadSmall.png'
import ignoreIcon from '../../assets/rejected.png'
import { UploadMyOwnTSL } from "./UploadMyOwnTSL";
import { CreateTSL } from "./CreateTSL";


export class UploadTestSpecification extends Component {

    constructor() {

        super()

        this.state = {
            acceptTSL: null,
            acceptDIC: null,
            acceptDLL: null,
            chosen: 1
        }

        this.createTSL = this.createTSL.bind(this)
        this.uploadTSL = this.uploadTSL.bind(this)
        this.ignoreStep = this.ignoreStep.bind(this)
        this.renderSwitch = this.renderSwitch.bind(this)
        this.renderChoice = this.renderChoice.bind(this)
        this.goBackToSelection = this.goBackToSelection.bind(this)
    }

    createTSL() {
        this.setState({ chosen: 3})
    }

    uploadTSL() {
        this.setState({ chosen: 2 })
    }

    ignoreStep() {
        this.props.handlerTest(this.state.acceptTSL, null, this.state.acceptDLL)
    }

    renderChoice() {
        return (
            <div>
                <Row>
                    <Col sm={4}>
                        <Row>
                            <div style={{ textAlign: "center", paddingTop: "20px" }}>
                                <h4>Create a TSL file</h4>
                                <AwesomeButton className="buttonAdd" type="primary" onPress={this.createTSL}><img style={{ marginRight: "15px" }} width="50" height="50" src={createIcon} alt="Logo" />Create TSL</AwesomeButton>
                            </div>
                        </Row>
                        <Row>
                            <div style={{ textAlign: "center", paddingTop: "40px" }}>
                                <h4>Upload your TSL file</h4>
                                <AwesomeButton className="buttonAdd" type="primary" onPress={this.uploadTSL}><img style={{ marginRight: "15px" }} width="50" height="50" src={uploadIcon} alt="Logo" />Upload TSL</AwesomeButton>
                            </div>

                        </Row>
                        <Row>
                            <div style={{ textAlign: "center", paddingTop: "60px" }}>
                                <h4>Or</h4>
                                <AwesomeButton className="buttonEdit" type="secondary" onPress={this.ignoreStep}><img style={{ marginRight: "15px" }} width="50" height="50" src={ignoreIcon} alt="Logo" />Ignore Step</AwesomeButton>
                            </div>
                        </Row>
                    </Col>
                    <Col sm={8}>
                        <Row>
                            <Figure style={{ padding: "50px 0px 0px 250px" }}>
                                <Figure.Image
                                    width={128}
                                    height={128}
                                    alt="128x128"
                                    src={InfoIcon}
                                />
                                <Figure.Caption>
                                    <h3>
                                        What is a TSL File?
                                    </h3>
                                    <div>
                                        A TSL, or Test Specific Language file, is a yaml file with the purpose of defining specific tests for your API, you can find it's detailed description in my GitHub
                                    </div>
                                </Figure.Caption>
                            </Figure>

                        </Row>
                    </Col>
                </Row>
                <div style={{ position: 'absolute', bottom: '0', width: '300px' }}>
                    <AwesomeButton style={{ marginBottom: '25px' }} type="primary" onPress={this.props.goBack}><img style={{ marginRight: "15px" }} width="50" height="50" src={backIcon} alt="Logo" />Go Back</AwesomeButton>
                </div>
            </div>
       )
    }

    goBackToSelection() {
        this.setState({chosen: 1})
    }

    renderSwitch() {
        switch (this.state.chosen) {
            case 1:
                return this.renderChoice()
            case 2:
                return <UploadMyOwnTSL
                    handlerTest={this.props.handlerTest}
                    goBackToSelection={this.goBackToSelection}
                />
            case 3:
                return <CreateTSL
                    handlerTest={this.props.handlerTest}
                    paths={this.props.paths}
                    servers={this.props.servers}
                    schemas={this.props.schemas}
                    schemasValues={this.props.schemasValues}
                    goBackToSelection={this.goBackToSelection}
                />
        }
    }

    render() {
        return (
            <div>
                {this.renderSwitch()}
            </div>
        )
    }
}