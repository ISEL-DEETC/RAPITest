import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Dropzone from '../../components/Dropzone'
import './UploadFile.css';
import { warningMessage } from '../../components/AlertComp'

export class UploadApiSpecification extends Component {

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
        if (reject.length !== 0 || accept.length > 1) {
            this.setState({ showWarning: true })
        }
        else {
            this.props.handlerAPI(accept[0])
        }
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    render() {
        return (
            <div>
                {this.state.showWarning ? warningMessage("Please upload only one yaml or json file", this.closeWarning) : <div></div> }
                <div className="root-dropzone">
                    <Dropzone
                        accept=".yaml, .json"
                        onDrop={this.onDrop}
                        history={this.props.history}
                        text={
                            <div align="center">
                                <p>Drop your API specification here, or click to select it.</p>
                                <p>Only .yaml files will be accepted</p>
                            </div>}
                    />
                </div>
             </div>
             
        )
    }
}