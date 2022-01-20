import React, { Component } from 'react';
import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap';
import Dropzone from '../../components/Dropzone'
import './UploadFile.css';
import authService from '../api-authorization/AuthorizeService'
import { warningMessage } from '../../components/AlertComp'
import Loader from 'react-loader-spinner'

export class UploadApiSpecification extends Component {

    constructor() {

        super()

        this.state = {
            showWarning: false,
            showVerify: false,
            step: 2
        }

        this.onDrop = this.onDrop.bind(this)
        this.closeWarning = this.closeWarning.bind(this)
        this.Upload = this.Upload.bind(this)
    }

    //callback for dropzone
    onDrop(accept, reject) {
        if (reject.length !== 0 || accept.length > 1) {
            this.setState({ showWarning: true })
        }
        else {
            this.Upload(accept[0])
        }
    }

    //upload
    async Upload(file) {
        this.setState({ showVerify: true })
        let data = new FormData();
        data.append('file', file);
        const token = await authService.getAccessToken();
        fetch(`UploadApiSpecification/UploadFile`, {
            method: 'POST',
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
            body: data
        }).then(res => {
            console.log("todo")
            //this.props.handler(this.state.step)
        })
    }

    closeWarning() {
        this.setState({ showWarning: false })
    }

    render() {
        return (
            <div>
                {this.state.showWarning ? warningMessage("Please upload only one yaml file", this.closeWarning) : <div></div> }
                <div className="root-dropzone">
                    <Dropzone
                        accept=".yaml"
                        onDrop={this.onDrop}
                        history={this.props.history}
                        text={
                            <div align="center">
                                <p>Drop your API specification here, or click to select it.</p>
                                <p>Only .yaml files will be accepted</p>
                            </div>}
                    />
                </div>
                {this.state.showVerify ? <div className="row">Uploading and verifying file integrity&nbsp;<Loader type="Grid" color="#00BFFF" height={35} width={35} /></div> : <div></div>}
             </div>
             
        )
    }
}