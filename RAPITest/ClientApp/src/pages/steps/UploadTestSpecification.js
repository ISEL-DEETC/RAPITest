import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Dropzone from '../../components/Dropzone'
import './UploadFile.css';
import { warningMessage } from '../../components/AlertComp'

export class UploadTestSpecification extends Component {

    constructor() {

        super()

        this.state = {
            showWarning: false
        }

        this.onDrop = this.onDrop.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
    }

    //callback for dropzone
    onDrop(accept, reject) {
        if (reject.length !== 0) {
            this.setState({ showWarning: true })
        }
        else {
            this.props.handlerTest(accept)
        }
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    render() {
        return (
            <div>
                {this.state.showWarning ? warningMessage("Please upload only yaml files", this.closeWarning) : <div></div>}
                <div className="root-dropzone">
                    <Dropzone
                        accept=".yaml"
                        onDrop={this.onDrop}
                        history={this.props.history}
                        text={
                            <div align="center">
                                <p>Drop your TSL files here, or click to select them.</p>
                                <p>Only .yaml files will be accepted</p>
                            </div>}
                    />
                </div>
            </div>

        )
    }
}