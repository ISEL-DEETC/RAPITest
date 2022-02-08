import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Dropzone from '../../components/Dropzone'
import { Container, Row,Col } from 'react-bootstrap'
import './UploadFile.css';
import { warningMessage } from '../../components/AlertComp'
import ListGroup from '../../components/ListGroup'

const acceptedSymbol = <svg className="bi bi-file-earmark-arrow-down" width="50" height="30" viewBox="0 0 16 16" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
    <path d="M4 1h5v1H4a1 1 0 00-1 1v10a1 1 0 001 1h8a1 1 0 001-1V6h1v7a2 2 0 01-2 2H4a2 2 0 01-2-2V3a2 2 0 012-2z" />
    <path d="M9 4.5V1l5 5h-3.5A1.5 1.5 0 019 4.5z" />
    <path fillRule="evenodd" d="M5.646 9.146a.5.5 0 01.708 0L8 10.793l1.646-1.647a.5.5 0 01.708.708l-2 2a.5.5 0 01-.708 0l-2-2a.5.5 0 010-.708z" clipRule="evenodd" />
    <path fillRule="evenodd" d="M8 6a.5.5 0 01.5.5v4a.5.5 0 01-1 0v-4A.5.5 0 018 6z" clipRule="evenodd" />
</svg>;

export class UploadTestSpecification extends Component {

    constructor() {

        super()

        this.state = {
            showWarning: false,
            warningMessage: null,
            acceptTSL: null,
            acceptDIC: null,
            acceptDLL: null
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
            this.setState({ acceptTSL: accept })
        }
    }

    onDropDIC(accept, reject) {
        if (reject.length !== 0 || accept.length > 1) {
            this.setState({ showWarning: true, warningMessage: "Please upload only one .txt file" })
        }
        else {
            this.setState({ acceptDIC: accept })
        }
    }

    onDropDLL(accept, reject) {
        if (reject.length !== 0) {
            this.setState({ showWarning: true, warningMessage: "Please upload only .dll files"  })
        }
        else {
            this.setState({ acceptDLL: accept })
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
        if (fileList.length === 1) {
            return
        }
        let title = "Accepted Files"
        return (
            <div style={{ borderRadius: "20px" }}>
                <div style={{ padding: "10px 10px 10px 10px" }}>
                    <ListGroup
                        title={title}
                        files={fileList}
                        symbol={acceptedSymbol}
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
                        {this.renderFiles()}
                    </Col>
                    <Col>
                        <button type="button" className="btn btn-outline-primary" onClick={this.continueCallback}>Continue</button>
                    </Col>
                </Row>
            </Container >

        )
    }
}