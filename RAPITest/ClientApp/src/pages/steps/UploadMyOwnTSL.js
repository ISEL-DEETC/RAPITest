import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Dropzone from '../../components/Dropzone'
import { Container, Row, Col } from 'react-bootstrap'
import './UploadFile.css';
import { warningMessage } from '../../components/AlertComp'
import ListGroupComp from '../../components/ListGroupComp'
import { AwesomeButton } from "react-awesome-button";
import "react-awesome-button/dist/styles.css";
import successIcon from '../../assets/tickSmall.png'

export class UploadMyOwnTSL extends Component {

    constructor() {

        super()

        this.state = {
            showWarning: false,
            warningMessage: null,
            acceptTSL: null,
            acceptDIC: null,
            acceptDLL: null,
            transitionTSL: false,
            transitionDIC: false,
            transitionDLL: false
        }

        this.onDropTSL = this.onDropTSL.bind(this)
        this.onDropDIC = this.onDropDIC.bind(this)
        this.onDropDLL = this.onDropDLL.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
        this.continueCallback = this.continueCallback.bind(this)
    }

    //callback for dropzone
    onDropTSL(accept, reject) {
        if (reject.length !== 0) {
            this.setState({ showWarning: true, warningMessage: "Please upload only .yaml files" })
        }
        else {
            this.setState({ acceptTSL: accept, transitionTSL: true })
        }
    }

    onDropDIC(accept, reject) {
        if (reject.length !== 0 || accept.length > 1) {
            this.setState({ showWarning: true, warningMessage: "Please upload only one .txt file" })
        }
        else {
            this.setState({ acceptDIC: accept, transitionDIC: true  })
        }
    }

    onDropDLL(accept, reject) {
        if (reject.length !== 0) {
            this.setState({ showWarning: true, warningMessage: "Please upload only .dll files" })
        }
        else {
            this.setState({ acceptDLL: accept, transitionDLL: true  })
        }
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    continueCallback() {
        if (this.state.acceptDIC === null) {
            this.props.handlerTest(this.state.acceptTSL, null, this.state.acceptDLL)
        } else {
            this.props.handlerTest(this.state.acceptTSL, this.state.acceptDIC[0], this.state.acceptDLL)
        }
    }

    toShow(file) {
        return (<div className="column">{file.name}</div>)
    }

    renderFiles() {
        let fileList = [...this.state.acceptTSL || [], ...this.state.acceptDIC || [], ...this.state.acceptDLL || []]
        let title = "Accepted Files"
        return (
            <div style={{ borderRadius: "20px" }}>
                <div style={{ padding: "10px 10px 10px 10px" }}>
                    <ListGroupComp
                        title={title}
                        files={fileList}
                        symbol={successIcon}
                        toShow={this.toShow}
                        history={this.props.history}
                    />
                </div>
            </div>
        )
    }

    render() {
        return (
            <Container >
                {this.state.showWarning ? warningMessage(this.state.warningMessage, this.closeWarning) : <div></div>}
                <Row>
                    <Col>
                        <div className="root-dropzone">
                            <Dropzone
                                accept=".yaml"
                                onDrop={this.onDropTSL}
                                history={this.props.history}
                                transition={this.state.transitionTSL}
                                text={
                                    <div align="center">
                                        <p>Drop your TSL files here, or click to select them.</p>
                                        <p>Only .yaml files will be accepted</p>
                                    </div>}
                            />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <div className="root-dropzone">
                            <Dropzone
                                accept=".txt"
                                onDrop={this.onDropDIC}
                                history={this.props.history}
                                transition={this.state.transitionDIC}
                                text={
                                    <div align="center">
                                        <p>Drop your Dictionary file here, or click to select it.</p>
                                        <p>Only .txt files will be accepted</p>
                                    </div>}
                            />
                        </div>
                    </Col>
                    <Col>
                        <div className="root-dropzone">
                            <Dropzone
                                accept=".dll"
                                onDrop={this.onDropDLL}
                                history={this.props.history}
                                transition={this.state.transitionDLL}
                                text={
                                    <div align="center">
                                        <p>Drop your DLL's here, or click to select them.</p>
                                        <p>Only .dll files will be accepted</p>
                                    </div>}
                            />
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        <div style={{ paddingTop: "75px", textAlign: "center" }}>
                            <AwesomeButton type="primary" disabled={this.state.acceptTSL === null} onPress={this.continueCallback}>Continue</AwesomeButton>
                        </div>
                    </Col>
                    <Col>
                        {this.renderFiles()}
                    </Col>
                </Row>
            </Container >

        )
    }

}